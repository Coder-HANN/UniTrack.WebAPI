using UniTrack.Domain.Enums;

namespace UniTrack.Application.Abstraction.Services.VerificationCode
{
    public interface IVerificationCodeService
    {
        // Kod oluşturur, Cache'e kaydeder ve Mail atar
        Task GenerateAndSendCodeAsync(string email, VerificationType type);

        // Gönderilen kodu kontrol eder (True/False döner)
        bool ValidateCode(string email, string code, VerificationType type);

        // İşlem bitince kodu siler (Güvenlik için)
        void RemoveCode(string email, VerificationType type);
    }

}
