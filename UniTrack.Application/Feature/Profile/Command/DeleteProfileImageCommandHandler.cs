using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.Storage;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Feature.Profile.Command
{
    public class DeleteProfileImageCommandHandler : IRequestHandler<DeleteProfileImageCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices _currentUserServices;
        private readonly IUserDetailRepository _userDetailRepository;
        private readonly IStorageService _storageService;
        private readonly ILocalizationService _localizationService;

        public DeleteProfileImageCommandHandler(
            ICurrentUserServices currentUserServices,
            IUserDetailRepository userDetailRepository,
            IStorageService storageService,
            ILocalizationService localizationService)
        {
            _currentUserServices = currentUserServices;
            _userDetailRepository = userDetailRepository;
            _storageService = storageService;
            _localizationService = localizationService;
        }

        public async Task<ServiceResponse<string>> Handle(DeleteProfileImageCommand request,CancellationToken cancellationToken)
        {
            var userId = _currentUserServices.CurrentUser();
            if (userId == null)
                return ServiceResponse<string>.Fail(
                    await _localizationService.Get(ValidationKeys.NotAuthorized));

            var userDetail = await _userDetailRepository.GetAsync(ud => ud.UserId == userId);
            if (userDetail == null)
                return ServiceResponse<string>.Fail(
                    await _localizationService.Get(ValidationKeys.UserNotFound));

            if (!string.IsNullOrWhiteSpace(userDetail.ProfileImageUrl))
                await _storageService.DeleteFileAsync(userDetail.ProfileImageUrl);

            userDetail.ProfileImageUrl = null;
            userDetail.UpdatedDate = DateTime.UtcNow;
            await _userDetailRepository.UpdateAsync(userDetail);

            return ServiceResponse<string>.Success(
                await _localizationService.Get(ValidationKeys.OperationSuccessful));
        }
    }
}