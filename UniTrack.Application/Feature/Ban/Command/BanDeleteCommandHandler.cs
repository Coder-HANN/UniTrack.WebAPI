using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Ban.Command
{
    public class BanDeleteCommandHandler : IRequestHandler<BanDeleteCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IBanRepository banRepository;
        private readonly ILocalizationService localizationService;
        public BanDeleteCommandHandler(
            ICurrentUserServices currentUserServices,
            IBanRepository banRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.banRepository = banRepository;
            this.localizationService = localizationService;
        }
        public async Task<ServiceResponse<string>> Handle(BanDeleteCommand request, CancellationToken cancellationToken)
        {
            var adminId = currentUserServices.CurrentUser();
            if (adminId == null || currentUserServices.Role() != Domain.Enums.Role.Admin)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = await localizationService.Get(Common.Constants.ValidationKeys.NotAuthorized)
                };
            }
            var existingBan = await banRepository.GetByIdAsync(request.BanId);
            if (existingBan == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = null
                };
            }
            existingBan.IsDeleted = true;
            await banRepository.UpdateAsync(existingBan);

            return new ServiceResponse<string>
            {
                IsSuccess = true,
                Data = null,
                Message = "işlem başarılı"
            };

        }
    }
}
