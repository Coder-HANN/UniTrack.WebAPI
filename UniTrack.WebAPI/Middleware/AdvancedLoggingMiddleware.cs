using Serilog.Context;
using System.Diagnostics;
using System.Security.Claims;
using UniTrack.Application.Abstraction.Services.Logger;


namespace UniTrack.WebAPI.Middleware
{
    public class AdvancedLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly LoggerServiceBase _logger;

        public AdvancedLoggingMiddleware(RequestDelegate next, LoggerServiceBase logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // =================================================================
            // 1. FİLTRELEME (Gereksiz 'Anonymous' Loglarını Engeller)
            // =================================================================
            // Tarayıcıların güvenlik kontrolü için attığı "OPTIONS" isteğini görmezden gel.
            if (context.Request.Method == "OPTIONS")
            {
                await _next(context);
                return;
            }

            // Resim, Swagger veya Favicon isteklerini görmezden gel.
            if (context.Request.Path.Value.Contains("favicon") ||
                context.Request.Path.Value.Contains("swagger") ||
                context.Request.Path.Value.StartsWith("/index.html"))
            {
                await _next(context);
                return;
            }

            // =================================================================
            // 2. KİMLİK TESPİTİ
            // =================================================================
            string currentIdentity = "Anonymous";
            string currentName = "Anonymous";

            // Sistem kullanıcıyı tanıdı mı? (Token geçerli mi?)
            bool isAuthenticated = context.User?.Identity?.IsAuthenticated ?? false;

            if (isAuthenticated)
            {
                // A) ID TESPİTİ
                var clubId = context.User.FindFirst("clubId")?.Value;
                var userId = context.User.FindFirst("userId")?.Value
                             ?? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? context.User.FindFirst("sub")?.Value;

                currentIdentity = clubId ?? userId ?? "Unknown ID";

                // B) İSİM TESPİTİ
                if (clubId != null)
                {
                    var cName = context.User.FindFirst("ClubName")?.Value;
                    currentName = cName ?? "Unknown Club";
                }
                else
                {
                    var name = context.User.FindFirst(ClaimTypes.Name)?.Value ?? "";
                    var surname = context.User.FindFirst(ClaimTypes.Surname)?.Value ?? "";

                    currentName = string.IsNullOrWhiteSpace(name) ? "Unknown User" : $"{name} {surname}".Trim();
                }
            }

            // =================================================================
            // 3. LOGLAMA VE HATA TESPİTİ
            // =================================================================
            var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";
            var device = context.Request.Headers["User-Agent"].ToString();

            using (LogContext.PushProperty("UserId", currentIdentity))
            using (LogContext.PushProperty("Name", currentName))
            using (LogContext.PushProperty("ClientIp", clientIp))
            using (LogContext.PushProperty("Device", device))
            {
                var stopwatch = Stopwatch.StartNew();
                try
                {
                    await _next(context);
                    stopwatch.Stop();

                    // --- BAŞARILI İSTEK ---
                    using (LogContext.PushProperty("AuditStatus", "Success"))
                    {
                        // Eğer hala Anonymous ise, bunun sebebini loga not düşüyoruz.
                        if (!isAuthenticated)
                        {
                            _logger.Warn($"ANONYMOUS İSTEK: {context.Request.Method} {context.Request.Path} - (Token Yok veya Geçersiz)");
                        }
                        else
                        {
                            _logger.Info($"İşlem Başarılı: {context.Request.Method} {context.Request.Path} - {stopwatch.ElapsedMilliseconds}ms");
                        }
                    }
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    // --- HATA ---
                    using (LogContext.PushProperty("AuditStatus", "Fail"))
                    {
                        _logger.Error($"SİSTEM HATASI: {context.Request.Method} {context.Request.Path}", ex);
                    }
                    throw;
                }
            }
        }
    }
}