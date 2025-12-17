using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Club.Query
{
    public class GetClubFollowerCountQuery : IRequest<ServiceResponse<long>>
    {
        public Guid ClubId { get; set; }
        public GetClubFollowerCountQuery(Guid clubId)
        {
            ClubId = clubId;
        }
        public GetClubFollowerCountQuery() { }
    }
}
