using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.ActiveDetail.Query
{
    public class GetActiveUsersFor30DaysQueryHandler : IRequestHandler<GetActiveUsersFor30DaysQuery, ServiceResponse<long>>
    {
        private readonly IUserRepository userRepository;
        public GetActiveUsersFor30DaysQueryHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        public async Task<ServiceResponse<long>> Handle(GetActiveUsersFor30DaysQuery request, CancellationToken cancellationToken)
        {
            
            var activeUserCount = await userRepository.CountAsync();
            if (activeUserCount == null)
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
