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

            if (request.File == null || request.File.Length == 0)
                return ServiceResponse<UploadProfileImageResponseDTO>.Fail("Görsel verisi boş.");

            try
            {
                using var stream = new MemoryStream();
                await request.File.CopyToAsync(stream);
                var bytes = stream.ToArray();

                var fileName = $"event-{clubId}-{Guid.NewGuid()}.png";
                var contentType = request.File.ContentType ?? "image/png";

                var url = await _storageService.UploadFileAsync(
                    bytes,
                    fileName,
                    StorageFileType.EventImage,
                    contentType);

                return ServiceResponse<UploadProfileImageResponseDTO>.Success(
                    "Görsel başarıyla yüklendi.",
                    new UploadProfileImageResponseDTO { Url = url });
            }
            catch (Exception)
            {
                return ServiceResponse<UploadProfileImageResponseDTO>.Fail("Görsel yüklenemedi.");
            }
        }
    }
}