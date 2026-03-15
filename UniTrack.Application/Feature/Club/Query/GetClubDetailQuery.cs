using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Club;

namespace UniTrack.Application.Feature.Club.Query
{
    public class GetClubDetailQuery : IRequest<ServiceResponse<GetClubDetailResponseDTO>>
    {
        public Guid ClubId { get; set; }

        public GetClubDetailQuery() { }
    }
}
