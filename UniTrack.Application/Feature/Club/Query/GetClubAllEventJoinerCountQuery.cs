using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Club.Query
{
    public class GetClubAllEventJoinerCountQuery : IRequest<ServiceResponse<int>>
    {
        public Guid ClubId { get; set; }
        public GetClubAllEventJoinerCountQuery() {}
    }
}
