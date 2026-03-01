using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Profile;

namespace UniTrack.Application.Feature.Profile.Query
{
    public class GetUserProfileQuery : IRequest<ServiceResponse<UserProfileUpdateResponseDTO>>
    {
        public GetUserProfileQuery() { }
    }
}
