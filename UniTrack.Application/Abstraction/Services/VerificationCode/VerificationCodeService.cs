using Microsoft.Extensions.Caching.Memory;
using UniTrack.Application.Abstraction.Services.Mail;
using UniTrack.Application.Abstraction.Services.VerificationCode;
using UniTrack.Domain.Enums;

namespace UniTrack.Infrastructure.Services // Sizin namespace'iniz farklı olabilir
{
    public class VerificationCodeService : IVerificationCodeService
    {
        private readonly IMemoryCache cache;
        private readonly IMailService mailService;

        public VerificationCodeService(IMemoryCache cache, IMailService mailService)
        {
            this.cache = cache;
            this.mailService = mailService;
        }

        public async Task GenerateAndSendCodeAsync(string email, VerificationType type)
        {
            // 1. 6 Haneli Kod Üret
            var code = new Random().Next(100000, 999999).ToString();

            // 2. Cache Key Oluştur (Örn: VERIFY_PasswordReset_ahmet@mail.com)
            var key = $"VERIFY_{type}_{email}";

            // 3. Cache'e kaydet (3 dakika ömürlü)
            cache.Set(key, code, TimeSpan.FromMinutes(3));

           
            await mailService.SendMailAsync(email, "Doğrulama Kodu", code);
        }

        public bool ValidateCode(string email, string code, VerificationType type)
        {
            var key = $"VERIFY_{type}_{email}";
            if (cache.TryGetValue(key, out string cachedCode))
            {
                return cachedCode == code;
            }
            return false;
        }

        public void RemoveCode(string email, VerificationType type)
        {
            var key = $"VERIFY_{type}_{email}";
            cache.Remove(key);
        }
    }
}