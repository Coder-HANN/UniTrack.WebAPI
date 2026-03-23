using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Profile;

namespace UniTrack.Application.Feature.Profile.Query
{
    public class GetClubSettingsQuery : IRequest<ServiceResponse<ClubSettingsResponseDTO>>
    {
        public GetClubSettingsQuery() { }
    }
}