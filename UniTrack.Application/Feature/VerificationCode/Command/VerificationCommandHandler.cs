using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.VerificationCode;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.VerificationCode.Command;
using UniTrack.Domain.Enums;

public class VerifyCommandHandler : IRequestHandler<VerificationCommand, ServiceResponse<string>>
{
    private readonly IClubRepository _clubRepository;
    private readonly IUserRepository _userRepository;
    private readonly IVerificationCodeService _codeService;
    private readonly ILocalizationService _localizationService;

    public VerifyCommandHandler(
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

    public async Task<ServiceResponse<string>> Handle(VerificationCommand request, CancellationToken cancellationToken)
    {
        if (request.VerificationType == VerificationType.ClubRegistration)
        {
            var club = await _clubRepository.GetByEmailAsync(request.Email);
            if (club == null)
                return new ServiceResponse<string> { IsSuccess = false, Message = await _localizationService.Get(ValidationKeys.ClubNotFound) };

            bool isValid = _codeService.ValidateCode(request.Email, request.VerificationCode, VerificationType.ClubRegistration);
            if (!isValid)
            {
                await _clubRepository.DeleteAsync(club);
                return new ServiceResponse<string> { IsSuccess = false, Message = await _localizationService.Get(ValidationKeys.InvalidOrExpiredCode) };
            }

            club.IsVerified = true;
            await _clubRepository.UpdateAsync(club);
            _codeService.RemoveCode(request.Email, VerificationType.ClubRegistration);

            return new ServiceResponse<string> { IsSuccess = true, Message = await _localizationService.Get(ValidationKeys.ClubVerifiedSuccess) };
        }

        if (request.VerificationType == VerificationType.UserRegistration)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
                return new ServiceResponse<string> { IsSuccess = false, Message = await _localizationService.Get(ValidationKeys.UserNotFound) };

            bool isValid = _codeService.ValidateCode(request.Email, request.VerificationCode, VerificationType.UserRegistration);
            if (!isValid)
            {
                await _userRepository.DeleteAsync(user);
                return new ServiceResponse<string> { IsSuccess = false, Message = await _localizationService.Get(ValidationKeys.InvalidOrExpiredCode) };
            }

            user.IsVerified = true;
            await _userRepository.UpdateAsync(user);
            _codeService.RemoveCode(request.Email, VerificationType.UserRegistration);

            return new ServiceResponse<string> { IsSuccess = true, Message = await _localizationService.Get(ValidationKeys.UserVerifiedSuccess) };
        }

        return new ServiceResponse<string> { IsSuccess = false, Message = await _localizationService.Get(ValidationKeys.InvalidRequest) };
    }
}