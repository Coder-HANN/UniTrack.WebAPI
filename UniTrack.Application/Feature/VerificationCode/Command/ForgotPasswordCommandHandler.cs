using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.VerificationCode;
using UniTrack.Application.Common;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.VerificationCode.Command
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ServiceResponse<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IClubRepository _clubRepository;
        private readonly IVerificationCodeService _codeService;

        public ForgotPasswordCommandHandler(IUserRepository userRepository, IVerificationCodeService codeService, IClubRepository clubRepository)
        {
            _userRepository = userRepository;
            _codeService = codeService;
            _clubRepository = clubRepository;
        }

        public async Task<ServiceResponse<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            // 1. Böyle bir kullanıcı var mı?
            var user = await _userRepository.GetByEmailAsync(request.Email);
            var club = await _clubRepository.GetByEmailAsync(request.Email);

            if (user == null || club == null)
            {
                // Güvenlik gereği "Böyle bir kullanıcı yok" dememek daha iyidir, ama geliştirme aşamasında dönebilirsiniz.
                return new ServiceResponse<string> { IsSuccess = false, Message = "Kullanıcı bulunamadı." };
            }

            // 2. Kod üret ve gönder (Tip: PasswordReset)
            await _codeService.GenerateAndSendCodeAsync(request.Email, VerificationType.PasswordReset);

            return new ServiceResponse<string> { IsSuccess = true, Message = "Doğrulama kodu e-posta adresinize gönderildi." };
        }
    }
}
