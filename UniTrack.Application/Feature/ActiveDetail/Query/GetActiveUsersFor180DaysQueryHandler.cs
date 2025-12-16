using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.ActiveDetail.Query
{
    public class GetActiveUsersFor180DaysQueryHandler : IRequestHandler<GetActiveUsersFor180DaysQuery, ServiceResponse<long>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IUserRepository userRepository;
        private readonly ILocalizationService localizationService;
        public GetActiveUsersFor180DaysQueryHandler(
            ICurrentUserServices currentUserServices,
            IUserRepository userRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.userRepository = userRepository;
            this.localizationService = localizationService;
        }
        public async Task<ServiceResponse<long>> Handle(GetActiveUsersFor180DaysQuery request, CancellationToken cancellationToken)
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

            var activeUsers = await userRepository.Get180DaysActiveUsersCountAsync();
            
            if (activeUsers == 0)
            {
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
                Data = activeUsers,
                Message = null
            };
        }
    }
}
