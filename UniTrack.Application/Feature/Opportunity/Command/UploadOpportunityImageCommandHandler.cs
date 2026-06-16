using MediatR;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.Storage;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Profile;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.OpportunityImage.Command
{
    public class UploadOpportunityImageCommandHandler
        : IRequestHandler<UploadOpportunityImageCommand, ServiceResponse<UploadProfileImageResponseDTO>>
    {
        private readonly ICurrentUserServices _currentUserServices;
        private readonly IStorageService _storageService;
        private readonly ILocalizationService _localizationService;

        public UploadOpportunityImageCommandHandler(
            ICurrentUserServices currentUserServices,
            IStorageService storageService,
            ILocalizationService localizationService)
        {
            _currentUserServices = currentUserServices;
            _storageService = storageService;
            _localizationService = localizationService;
        }

        public async Task<ServiceResponse<UploadProfileImageResponseDTO>> Handle(UploadOpportunityImageCommand request, CancellationToken cancellationToken)
        {
            // Admin rol kontrolü
            var isAdmin = _currentUserServices.Role();
            if (isAdmin != Role.Admin && isAdmin != Role.SuperAdmin)
                return ServiceResponse<UploadProfileImageResponseDTO>.Fail(await _localizationService.Get(ValidationKeys.NotAuthorized));

            try
            {
                using var stream = new MemoryStream();
                await request.File.CopyToAsync(stream);
                var bytes = stream.ToArray();

                var fileName = $"opportunity-{Guid.NewGuid()}.png";
                var contentType = request.File.ContentType ?? "image/png";

                var url = await _storageService.UploadFileAsync(
                    bytes,
                    fileName,
                    StorageFileType.OpportunityImage,
                    contentType);

                return ServiceResponse<UploadProfileImageResponseDTO>.Success(
                    await _localizationService.Get(ValidationKeys.OperationSuccessful),
                    new UploadProfileImageResponseDTO { Url = url });
            }
            catch (Exception)
            {
                return ServiceResponse<UploadProfileImageResponseDTO>.Fail(
                    await _localizationService.Get("Görsel yüklenemedi."));
            }
        }
    }
}