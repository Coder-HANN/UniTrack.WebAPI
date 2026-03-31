using MediatR;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.Storage;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Profile;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.EventImage.Command
{
    public class UploadEventImageCommandHandler : IRequestHandler<UploadEventImageCommand, ServiceResponse<UploadProfileImageResponseDTO>>
    {
        private readonly ICurrentUserServices _currentUserServices;
        private readonly IStorageService _storageService;
        private readonly ILocalizationService _localizationService;

        public UploadEventImageCommandHandler(
            ICurrentUserServices currentUserServices,
            IStorageService storageService,
            ILocalizationService localizationService)
        {
            _currentUserServices = currentUserServices;
            _storageService = storageService;
            _localizationService = localizationService;
        }

        public async Task<ServiceResponse<UploadProfileImageResponseDTO>> Handle(
            UploadEventImageCommand request, CancellationToken cancellationToken)
        {
            var clubId = _currentUserServices.CurrentClub();
            if (clubId == null)
                return ServiceResponse<UploadProfileImageResponseDTO>.Fail(
                    await _localizationService.Get(ValidationKeys.NotAuthorized));

            if (string.IsNullOrWhiteSpace(request.Base64))
                return ServiceResponse<UploadProfileImageResponseDTO>.Fail("Görsel verisi boş.");

            try
            {
                var base64 = request.Base64.Contains(',')
                    ? request.Base64.Split(',')[1]
                    : request.Base64;

                var bytes = Convert.FromBase64String(base64);
                var fileName = $"event-{clubId}-{Guid.NewGuid()}.png";

                var url = await _storageService.UploadFileAsync(
                    bytes, fileName, StorageFileType.EventImage, "image/png");

                return ServiceResponse<UploadProfileImageResponseDTO>.Success(
                    "Görsel başarıyla yüklendi.",
                    new UploadProfileImageResponseDTO { Url = url });
            }
            catch (FormatException)
            {
                return ServiceResponse<UploadProfileImageResponseDTO>.Fail("Görsel formatı geçersiz.");
            }
            catch (Exception)
            {
                return ServiceResponse<UploadProfileImageResponseDTO>.Fail("Görsel yüklenemedi.");
            }
        }
    }
}