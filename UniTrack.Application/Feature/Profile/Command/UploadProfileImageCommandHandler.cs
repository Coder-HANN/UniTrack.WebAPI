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

            if (request.File == null || request.File.Length == 0)
                return ServiceResponse<UploadProfileImageResponseDTO>.Fail("Görsel verisi boş.");

            try
            {
                using var stream = new MemoryStream();
                await request.File.CopyToAsync(stream);
                var bytes = stream.ToArray();
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