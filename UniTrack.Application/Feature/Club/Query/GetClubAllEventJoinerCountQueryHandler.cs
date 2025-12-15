using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Club.Query
{
    public class GetClubAllEventJoinerCountQueryHandler : IRequestHandler<GetClubAllEventJoinerCountQuery, ServiceResponse<int>>
    {
        private readonly IEventUserRepository eventUserRepository;
        private readonly ICurrentUserServices currentUserService;
        public GetClubAllEventJoinerCountQueryHandler(IEventUserRepository eventUserRepository, ICurrentUserServices currentUserService)
        {
            this.eventUserRepository = eventUserRepository;
            this.currentUserService = currentUserService;
        }
        public async Task<ServiceResponse<int>> Handle(GetClubAllEventJoinerCountQuery request, CancellationToken cancellationToken)
        {
            var clubId = currentUserService.CurrentClub();

            if(clubId == null || clubId != request.ClubId)
            {
                return new ServiceResponse<int>
                {
                    IsSuccess = false,
                    Message = "You are not authorized to access this club's data.",
                    Data = 0
                };
            }

            var joinerCount = await eventUserRepository.GetTotalJoinerCountByClubIdAsync(request.ClubId);

            if (joinerCount == 0 || joinerCount == null)
            { 
                return new ServiceResponse<int>
                {
                    IsSuccess = false,
                    Message = "No joiners found for the club's events.",
                    Data = 0
                };
            }

            return new ServiceResponse<int>
            {
                IsSuccess = true,
                Message = "Total joiner count retrieved successfully.",
                Data = joinerCount
            };
        }
    }
}
