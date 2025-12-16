using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.ActiveDetail.Query
{
    public class GetActiveUsersFor90DaysQueryHandler : IRequestHandler<GetActiveUsersFor90DaysQuery, ServiceResponse<long>>
    {
        private readonly IUserRepository userRepository;
        private readonly ICurrentUserServices currentUserServices;
        private readonly ILocalizationService localizationService;
        public GetActiveUsersFor90DaysQueryHandler(IUserRepository userRepository, ICurrentUserServices currentUserServices, ILocalizationService localizationService)
        {
            this.userRepository = userRepository;
            this.currentUserServices = currentUserServices;
            this.localizationService = localizationService;
        }
        public async Task<ServiceResponse<long>> Handle(GetActiveUsersFor90DaysQuery request, CancellationToken cancellationToken)
        {
            var role = currentUserServices.Role();
            if (role == null || role != Domain.Enums.Role.Admin)
            {
                return new ServiceResponse<long>
                {
                    IsSuccess = false,
                    Data = 0,
                    Message = await localizationService.Get(Common.Constants.ValidationKeys.NotAuthorized)
                };
            }

            var activeUserCount = await userRepository.For90DaysCountAsync();

            if (activeUserCount == 0) {
                return new ServiceResponse<long>
                {
                    IsSuccess = false,
                    Data = 0,
                    Message = null
                };
            }
            return new ServiceResponse<long>
            {
                IsSuccess = true,
                Data = activeUserCount,
                Message = null
            };
        }
    }
}
