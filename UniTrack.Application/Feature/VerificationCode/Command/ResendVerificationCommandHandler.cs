using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.VerificationCode;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.VerificationCode.Command;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.VerificationCode.Command
{
    public class ResendVerificationCommandHandler : IRequestHandler<ResendVerificationCommand, ServiceResponse<string>>
    {
        private readonly IClubRepository _clubRepository;
        private readonly IUserRepository _userRepository;
        private readonly IVerificationCodeService _codeService;
        private readonly ILocalizationService _localizationService;

        public ResendVerificationCommandHandler(
            IClubRepository clubRepository,
            IUserRepository userRepository,
            IVerificationCodeService codeService,
            ILocalizationService localizationService)
        {
            _clubRepository = clubRepository;
            _userRepository = userRepository;
            _codeService = codeService;
            _localizationService = localizationService;
        }

        public async Task<ServiceResponse<string>> Handle(ResendVerificationCommand request, CancellationToken cancellationToken)
        {
            if (request.VerificationType == VerificationType.ClubRegistration)
            {
                var club = await _clubRepository.GetByEmailAsync(request.Email);
                if (club == null)
                    return new ServiceResponse<string> { IsSuccess = false, Message = await _localizationService.Get(ValidationKeys.ClubNotFound) };

                if (club.IsVerified)
                    return new ServiceResponse<string> { IsSuccess = false, Message = await _localizationService.Get(ValidationKeys.ClubAlreadyVerified) };

                _codeService.RemoveCode(request.Email, VerificationType.ClubRegistration);
                await _codeService.GenerateAndSendCodeAsync(request.Email, VerificationType.ClubRegistration);

                return new ServiceResponse<string> { IsSuccess = true, Message = await _localizationService.Get(ValidationKeys.VerificationCodeResentSuccess) };
            }

            if (request.VerificationType == VerificationType.UserRegistration)
            {
                var user = await _userRepository.GetByEmailAsync(request.Email);
                if (user == null)
                    return new ServiceResponse<string> { IsSuccess = false, Message = await _localizationService.Get(ValidationKeys.UserNotFound) };

                if (user.IsVerified)
                    return new ServiceResponse<string> { IsSuccess = false, Message = await _localizationService.Get(ValidationKeys.UserAlreadyVerified) };

                _codeService.RemoveCode(request.Email, VerificationType.UserRegistration);
                await _codeService.GenerateAndSendCodeAsync(request.Email, VerificationType.UserRegistration);

                return new ServiceResponse<string> { IsSuccess = true, Message = await _localizationService.Get(ValidationKeys.VerificationCodeResentSuccess) };
            }

            return new ServiceResponse<string> { IsSuccess = false, Message = await _localizationService.Get(ValidationKeys.InvalidRequest) };
        }
    }
}