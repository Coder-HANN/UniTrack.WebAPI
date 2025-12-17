using MediatR;
using Microsoft.AspNetCore.Identity;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.VerificationCode;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.VerificationCode.Command
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ServiceResponse<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IVerificationCodeService _codeService;
        private readonly IPasswordHasher<User> passwordHasher;
        private readonly ILocalizationService localizationService;


        public ResetPasswordCommandHandler(IUserRepository userRepository, IVerificationCodeService codeService, IPasswordHasher<User> passwordHasher, ILocalizationService localizationService)
        {
            _userRepository = userRepository;
            _codeService = codeService;
            this.passwordHasher = passwordHasher;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            // 1. Kodu Doğrula (Veritabanına gitmeden önce kodu kontrol etmek performansı artırır)
            bool isValid = _codeService.ValidateCode(request.Email, request.Code, VerificationType.PasswordReset);

            if (!isValid)
            {
                return new ServiceResponse<string> { IsSuccess = false, Message = await localizationService.Get(ValidationKeys.InvalidOrExpiredCode) };
            }

            // 2. Kullanıcıyı getir
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null) return new ServiceResponse<string> { IsSuccess = false, Message = await localizationService.Get(ValidationKeys.UserNotFound) };

            // 3. Şifreyi Güncelle (Hashleme işlemi burada yapılmalı)
            var hashedPassword = passwordHasher.HashPassword(null, request.NewPassword);
            user.Password = hashedPassword;

            await _userRepository.UpdateAsync(user);

            // 4. Kodu Sil
            _codeService.RemoveCode(request.Email, VerificationType.PasswordReset);

            return new ServiceResponse<string> { IsSuccess = true, Message = await localizationService.Get(ValidationKeys.PasswordChangedSuccess) };
        }
    }
}
