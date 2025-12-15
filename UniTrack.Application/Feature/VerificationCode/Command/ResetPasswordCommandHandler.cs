using MediatR;
using Microsoft.AspNetCore.Identity;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.VerificationCode;
using UniTrack.Application.Common;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.VerificationCode.Command
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ServiceResponse<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IVerificationCodeService _codeService;
        private readonly IPasswordHasher<User> passwordHasher;


        public ResetPasswordCommandHandler(IUserRepository userRepository, IVerificationCodeService codeService, IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _codeService = codeService;
            this.passwordHasher = passwordHasher;
        }

        public async Task<ServiceResponse<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            // 1. Kodu Doğrula (Veritabanına gitmeden önce kodu kontrol etmek performansı artırır)
            bool isValid = _codeService.ValidateCode(request.Email, request.Code, VerificationType.PasswordReset);

            if (!isValid)
            {
                return new ServiceResponse<string> { IsSuccess = false, Message = "Kod geçersiz." };
            }

            // 2. Kullanıcıyı getir
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null) return new ServiceResponse<string> { IsSuccess = false, Message = "Kullanıcı bulunamadı." };

            // 3. Şifreyi Güncelle (Hashleme işlemi burada yapılmalı)
            var hashedPassword = passwordHasher.HashPassword(null, request.NewPassword);
            user.Password = hashedPassword;

            await _userRepository.UpdateAsync(user);

            // 4. Kodu Sil
            _codeService.RemoveCode(request.Email, VerificationType.PasswordReset);

            return new ServiceResponse<string> { IsSuccess = true, Message = "Şifreniz başarıyla değiştirildi." };
        }
    }
}
