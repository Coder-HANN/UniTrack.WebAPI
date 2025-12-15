using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Club.Query
{
    public class GetFollowClubCountQueryHandler : IRequestHandler<GetFollowClubCountQuery, ServiceResponse<int>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IUserClubRepository userClubRepository;
        public GetFollowClubCountQueryHandler(
            ICurrentUserServices currentUserServices,
            IUserClubRepository userClubRepository)
        {
            this.currentUserServices = currentUserServices;
            this.userClubRepository = userClubRepository;
        }
        public async Task<ServiceResponse<int>> Handle(GetFollowClubCountQuery request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            if (userId == null)
            {
                return new ServiceResponse<int>
                {
                    IsSuccess = false,
                    Data = 0,
                    Message = "Unauthorized"
                };
            }

            var followClubCount = await userClubRepository.GetFollowedClubCountAsync(userId);

            return new ServiceResponse<int>
            {
                IsSuccess = true,
                Data =  followClubCount,
                Message = "Followed club count retrieved successfully"
            };

        }
    }
}
