using MediatR;
using Microsoft.AspNetCore.Identity;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.VerificationCode;
using UniTrack.Application.Common;
using UniTrack.Domain.Entities; // Club entity'sinin burada olduğunu varsayıyorum
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.VerificationCode.Command
{
    public class ResetClubPasswordCommandHandler : IRequestHandler<ResetClubPasswordCommand, ServiceResponse<string>>
    {
        private readonly IClubRepository _clubRepository;
        private readonly IVerificationCodeService _codeService;
        private readonly IPasswordHasher<Domain.Entities.Club> _passwordHasher; // <User> yerine <Club>

        public ResetClubPasswordCommandHandler(
            IClubRepository clubRepository,
            IVerificationCodeService codeService,
            IPasswordHasher<Domain.Entities.Club> passwordHasher)
        {
            _clubRepository = clubRepository;
            _codeService = codeService;
            _passwordHasher = passwordHasher;
        }

        public async Task<ServiceResponse<string>> Handle(ResetClubPasswordCommand request, CancellationToken cancellationToken)
        {
            // 1. Kodu Doğrula (Tip: PasswordReset - User ile aynı tipi kullanabilir veya ayırabilirsiniz)
            bool isValid = _codeService.ValidateCode(request.Email, request.Code, VerificationType.PasswordReset);

            if (!isValid)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = "Kod geçersiz veya süresi dolmuş."
                };
            }

            // 2. Kulübü getir
            // Not: Kulüplerin genellikle "PresidentMail" veya kayıt oldukları özel bir mail alanı vardır.
            // Repository metodunuzun bu mail alanına baktığından emin olun.
            var club = await _clubRepository.GetByEmailAsync(request.Email);

            if (club == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = "Bu e-posta adresine ait bir kulüp bulunamadı."
                };
            }

            // 3. Şifreyi Hashle ve GÜNCELLE
            // DİKKAT: User kodunuzda hash'i oluşturmuşsunuz ama user nesnesine atamayı unutmuşsunuz.
            // Burada o atamayı yapıyoruz:
            var hashedPassword = _passwordHasher.HashPassword(club, request.NewPassword);

            club.Password = hashedPassword; // Kulüp entity'sindeki şifre alanı

            // 4. Veritabanını güncelle
            await _clubRepository.UpdateAsync(club);

            // 5. Kodu Sil (Güvenlik için tek kullanımlık yapıyoruz)
            _codeService.RemoveCode(request.Email, VerificationType.PasswordReset);

            return new ServiceResponse<string>
            {
                IsSuccess = true,
                Message = "Kulüp şifresi başarıyla güncellendi."
            };
        }
    }
}