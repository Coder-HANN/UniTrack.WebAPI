using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Profile;

namespace UniTrack.Application.Feature.Profile.Query
{
    public class GetClubProfileQuery : IRequest<ServiceResponse<GetClubProfileResponseDTO>>
    {
    }
}
