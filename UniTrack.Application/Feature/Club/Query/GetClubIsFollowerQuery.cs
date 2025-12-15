using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Club;

namespace UniTrack.Application.Feature.Club.Query
{
    public class GetClubIsFollowerQuery : IRequest<ServiceResponse<List<GetClubIsFollowerQueryResponseDTO>>>
    {
        public Guid ClubId { get; set; }
       
        public GetClubIsFollowerQuery(Guid clubId)
        {
            ClubId = clubId;
            
        }
        public GetClubIsFollowerQuery() { }
    }
}
