using MediatR;
using Microsoft.AspNetCore.Identity;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.VerificationCode;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.VerificationCode.Command
{
    public class ResetClubPasswordCommandHandler : IRequestHandler<ResetClubPasswordCommand, ServiceResponse<string>>
    {
        private readonly IClubRepository _clubRepository;
        private readonly IVerificationCodeService _codeService;
        private readonly IPasswordHasher<Domain.Entities.Club> _passwordHasher;
        private readonly ILocalizationService localizationService;

        public ResetClubPasswordCommandHandler(
            IClubRepository clubRepository,
            IVerificationCodeService codeService,
            IPasswordHasher<Domain.Entities.Club> passwordHasher,
            ILocalizationService localizationService)
        {
            _clubRepository = clubRepository;
            _codeService = codeService;
            _passwordHasher = passwordHasher;
            this.localizationService = localizationService;
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
                    Message = await localizationService.Get(ValidationKeys.InvalidOrExpiredCode)
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
                    Message =await localizationService.Get(ValidationKeys.ClubNotFound)
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
                Message = await localizationService.Get(ValidationKeys.ClubPasswordResetSuccess)
            };
        }
    }
}