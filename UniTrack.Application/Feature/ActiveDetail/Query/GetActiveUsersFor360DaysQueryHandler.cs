using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.ActiveDetail.Query
{
    public class GetActiveUsersFor360DaysQueryHandler : IRequestHandler<GetActiveUsersFor360DaysQuery, ServiceResponse<long>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IUserRepository userRepository;
        public GetActiveUsersFor360DaysQueryHandler(
            ICurrentUserServices currentUserServices,
            IUserRepository userRepository)
        {
            this.currentUserServices = currentUserServices;
            this.userRepository = userRepository;
        }
        public async Task<ServiceResponse<long>> Handle(GetActiveUsersFor360DaysQuery request, CancellationToken cancellationToken)
        {
            var role = currentUserServices.Role();
            if (role == null || role != Domain.Enums.Role.Admin)
            {
                return new ServiceResponse<long>
                {
                    IsSuccess = false,
                    Data = 0,
                    Message = "Yetkisiz kullanıcı"
                };
            }
            var activeUserCount = await userRepository.Get360DaysActiveUsersCountAsync();
            if (activeUserCount == 0)
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
                Data = activeUserCount,
                Message = null
            };
        }
    }
}
