using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.VerificationCode;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.VerificationCode.Command
{
    public class ResendVerificationCommandHandler : IRequestHandler<ResendVerificationCommand, ServiceResponse<string>>
    {
        private readonly IClubRepository _clubRepository;
        private readonly ICurrentUserServices _currentUserServices;
        private readonly IVerificationCodeService _codeService;
        private readonly ILocalizationService _localizationService;

        public ResendVerificationCommandHandler(
            IClubRepository clubRepository,
            IVerificationCodeService codeService,
            ILocalizationService localizationService,
            ICurrentUserServices currentUserServices)
        {
            _clubRepository = clubRepository;
            _codeService = codeService;
            _localizationService = localizationService;
            _currentUserServices = currentUserServices;
        }

        public async Task<ServiceResponse<string>> Handle(ResendVerificationCommand request, CancellationToken cancellationToken)
        {
            // 1. Yetki Kontrolü
            var clubId = _currentUserServices.CurrentClub();
            var role = _currentUserServices.Role();

            if (clubId == null || role == null || role == Role.User)
            {
                return new ServiceResponse<string> { IsSuccess = false, Message = await _localizationService.Get(ValidationKeys.NotAuthorized) };
            }

            // 2. Kulübü Getir
            var club = await _clubRepository.GetByIdAsync(clubId.Value);
            if (club == null)
            {
                return new ServiceResponse<string> { IsSuccess = false, Message = await _localizationService.Get(ValidationKeys.ClubNotFound) };
            }

            // 3. Durum Kontrolü
            if (club.IsVerified)
            {
                return new ServiceResponse<string> { IsSuccess = false, Message = await _localizationService.Get(ValidationKeys.ClubAlreadyVerified) };
            }
            
            // 4. Eski Kodu Sil ve Yeni Kodu Gönder
            // Tek satırla hem cache temizliği hem mail gönderimi yapmış oluyoruz
            _codeService.RemoveCode(club.PresidentMail, VerificationType.ClubRegistration);
            await _codeService.GenerateAndSendCodeAsync(club.PresidentMail, VerificationType.ClubRegistration);

            return new ServiceResponse<string>
            {
                IsSuccess = true,
                Message = await _localizationService.Get(ValidationKeys.VerificationCodeResentSuccess)
            };
        }
    }
}