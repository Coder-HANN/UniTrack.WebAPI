using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
using UniTrack.Domain.Enums;


namespace UniTrack.WebAPI.Middleware
{
    public class UserMiddleware
    {
        private readonly RequestDelegate _next;

        public UserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();

            // Giriş/Kayıt yollarını atla
            if (path != null && (path.Contains("login") || (path.Contains("register") && !path.Contains("adminregister"))))
            {
                Console.WriteLine($"⏭️ UserMiddleware atlandı: {path}");
            }
            else // Diğer yollar
            {

                if (context.User.Identity?.IsAuthenticated == true)
                {
                    // userId'yi context.Items'a ekle
                    var userIdClaim = context.User.Claims.FirstOrDefault(c =>
                        c.Type.Equals("userId", StringComparison.OrdinalIgnoreCase));

                    if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
                    {
                        // Artık Guid tipinde ekleniyor
                        context.Items["userId"] = userId;
                    }
                    else
                    {
                        Console.WriteLine("🟠 UserId claim bulunamadı.");
                    }

                    // clubId'yi context.Items'a ekle (isteğe bağlı, clubId claim varsa)
                    var clubIdClaim = context.User.Claims.FirstOrDefault(c =>
                        c.Type.Equals("clubId", StringComparison.OrdinalIgnoreCase));

                    if (clubIdClaim != null && Guid.TryParse(clubIdClaim.Value, out var clubId))
                    {
                        // Artık Guid tipinde ekleniyor
                        context.Items["clubId"] = clubId;
                    }

                    var roleClaim = context.User.Claims.FirstOrDefault(c =>
                        c.Type.Equals(ClaimTypes.Role, StringComparison.OrdinalIgnoreCase));

                    if (roleClaim != null)
                    {
                        if (Enum.TryParse<Role>(roleClaim.Value, true, out var role))
                        {
                            context.Items["role"] = role;
                        }
                    }

                    var universityIdClaim = context.User.Claims.FirstOrDefault(c =>
                        c.Type.Equals("universityId", StringComparison.OrdinalIgnoreCase));
                    if (universityIdClaim != null && Guid.TryParse(universityIdClaim.Value, out var universityId))
                    {
                        context.Items["universityId"] = universityId;
                    }
                }
                else
                {
                    Console.WriteLine("🔴 Kullanıcı authenticate değil.");
                }
            }

            await _next(context);
        }
    }
}