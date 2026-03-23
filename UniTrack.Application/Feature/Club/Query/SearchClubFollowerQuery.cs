using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Club;

namespace UniTrack.Application.Feature.Club.Query
{
    public class SearchClubFollowerQuery : IRequest<ServiceResponse<List<ClubFollowerSearchResponseDTO>>>
    {
        public string Name { get; set; }
    }
}