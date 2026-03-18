using MediatR;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.Storage;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Profile;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Profile.Command
{
    public class UploadProfileImageCommandHandler : IRequestHandler<UploadProfileImageCommand, ServiceResponse<UploadProfileImageResponseDTO>>
    {
        private readonly ICurrentUserServices _currentUserServices;
        private readonly IStorageService _storageService;
        private readonly ILocalizationService _localizationService;

        public UploadProfileImageCommandHandler(
            ICurrentUserServices currentUserServices,
            IStorageService storageService,
            ILocalizationService localizationService)
        {
            _currentUserServices = currentUserServices;
            _storageService = storageService;
            _localizationService = localizationService;
        }

        public async Task<ServiceResponse<UploadProfileImageResponseDTO>> Handle(UploadProfileImageCommand request,CancellationToken cancellationToken)
        {
            var userId = _currentUserServices.CurrentUser();
            if (userId == null)
                return ServiceResponse<UploadProfileImageResponseDTO>.Fail(
                    await _localizationService.Get(ValidationKeys.NotAuthorized));

            if (string.IsNullOrWhiteSpace(request.Base64))
                return ServiceResponse<UploadProfileImageResponseDTO>.Fail("Görsel verisi boş.");

            try
            {
                // data:image/png;base64,... formatından saf base64 al
                var base64 = request.Base64.Contains(',')
                    ? request.Base64.Split(',')[1]
                    : request.Base64;

                var bytes = Convert.FromBase64String(base64);
                var fileName = $"user-{userId}-{Guid.NewGuid()}.png";

                var url = await _storageService.UploadFileAsync(
                    bytes,
                    fileName,
                    StorageFileType.User,
                    "image/png");

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