using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Domain.Enums;

public class BanMiddleware
{
    private readonly RequestDelegate _next;

    public BanMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IBanRepository banRepository)
    {
        var path = context.Request.Path.Value?.ToLower();

        // 1. Giriş/Kayıt yollarını atla
        if (path != null && (path.Contains("login") || path.Contains("register")))
        {
            await _next(context);
            return;
        }

        // 2. Kimliği doğrulanmamış kullanıcıları atla (Ban kontrolüne gerek yok)
        if (context.User.Identity?.IsAuthenticated != true)
        {
            await _next(context);
            return;
        }

        // 3. Claim'leri al
        var userIdClaim = context.User.Claims.FirstOrDefault(c => c.Type.Equals("userId", StringComparison.OrdinalIgnoreCase));
        var clubIdClaim = context.User.Claims.FirstOrDefault(c => c.Type.Equals("clubId", StringComparison.OrdinalIgnoreCase));

        // Kullanıcı ban kontrolü
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            await banRepository.LiftBanIfExpiredAsync(userId, Role.User);

            if (await banRepository.IsBannedAsync(userId, Role.User))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Kullanıcı hesabınız banlanmıştır.");
                return; // İstek zincirini burada sonlandır
            }
        }

        // Kulüp ban kontrolü
        if (clubIdClaim != null && Guid.TryParse(clubIdClaim.Value, out var clubId))
        {
            await banRepository.LiftBanIfExpiredAsync(clubId, Role.Club);

            if (await banRepository.IsBannedAsync(clubId, Role.Club))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Kulüp hesabınız banlanmıştır.");
                return; // İstek zincirini burada sonlandır
            }
        }
        // Admin ban kontrolü
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var adminId))
        {
            await banRepository.LiftBanIfExpiredAsync(adminId, Role.Admin);
            if (await banRepository.IsBannedAsync(adminId, Role.Admin))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Admin hesabınız dondurulmuştur,Lütfen yetkiliyle iletişime geçiniz.");
                return;
            }
        }

        // Ban yoksa veya kontrol edilemediyse zinciri devam ettir
        await _next(context);
    }
}