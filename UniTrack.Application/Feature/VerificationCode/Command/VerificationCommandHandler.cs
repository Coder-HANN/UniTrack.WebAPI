using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.VerificationCode;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.VerificationCode.Command;
using UniTrack.Domain.Enums; 

public class VerifyClubCommandHandler : IRequestHandler<VerificationCommand, ServiceResponse<string>>
{
    private readonly IClubRepository _clubRepository;
    private readonly IVerificationCodeService _codeService;
    private readonly ILocalizationService localizationService;

    public VerifyClubCommandHandler(IClubRepository clubRepository, IVerificationCodeService codeService, ILocalizationService localizationService)
    {
        _clubRepository = clubRepository;
        _codeService = codeService;
        this.localizationService = localizationService;
    }

    public async Task<ServiceResponse<string>> Handle(VerificationCommand request, CancellationToken cancellationToken)
    {
        // 1. Kulübü bul
        var club = await _clubRepository.GetByEmailAsync(request.Email);
        if (club == null)
            return new ServiceResponse<string> { IsSuccess = false, Message = await localizationService.Get(ValidationKeys.ClubNotFound) };

        // 2. Kodu Generic Servis ile Kontrol Et (Tip: ClubRegistration)
        bool isValid = _codeService.ValidateCode(request.Email, request.VerificationCode, VerificationType.ClubRegistration);

        if (!isValid)
        {

            await _clubRepository.DeleteAsync(club);
            return new ServiceResponse<string> { IsSuccess = false, Message = await localizationService.Get(ValidationKeys.InvalidOrExpiredCode) };
        }

        // 3. Başarılı ise durumu güncelle
        club.IsVerified = true;
        await _clubRepository.UpdateAsync(club);

        // 4. Kodu temizle (Tekrar kullanılamasın)
        _codeService.RemoveCode(request.Email, VerificationType.ClubRegistration);

        return new ServiceResponse<string> { IsSuccess = true, Message = await localizationService.Get(ValidationKeys.ClubVerifiedSuccess) };
    }
}