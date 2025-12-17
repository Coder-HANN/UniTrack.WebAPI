using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.VerificationCode;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.VerificationCode.Command
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ServiceResponse<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IClubRepository _clubRepository;
        private readonly IVerificationCodeService _codeService;
        private readonly ILocalizationService localizationService;

        public ForgotPasswordCommandHandler(IUserRepository userRepository, IVerificationCodeService codeService, IClubRepository clubRepository, ILocalizationService localizationService)
        {
            _userRepository = userRepository;
            _codeService = codeService;
            _clubRepository = clubRepository;
            this.localizationService = localizationService;

        }

        public async Task<ServiceResponse<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            // 1. Böyle bir kullanıcı var mı?
            var user = await _userRepository.GetByEmailAsync(request.Email);
            var club = await _clubRepository.GetByEmailAsync(request.Email);

            if (user == null || club == null)
            {
                // Güvenlik gereği "Böyle bir kullanıcı yok" dememek daha iyidir, ama geliştirme aşamasında dönebilirsiniz.
                return new ServiceResponse<string> { IsSuccess = false, Message = await localizationService.Get(ValidationKeys.NotAuthorized) };
            }

            // 2. Kod üret ve gönder (Tip: PasswordReset)
            await _codeService.GenerateAndSendCodeAsync(request.Email, VerificationType.PasswordReset);

            return new ServiceResponse<string> { IsSuccess = true, Message = await localizationService.Get(ValidationKeys.VerificationCodeSent) };
        }
    }
}
