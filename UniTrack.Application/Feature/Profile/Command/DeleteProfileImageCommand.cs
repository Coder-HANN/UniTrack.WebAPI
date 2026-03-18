using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Profile.Command
{
    public class DeleteProfileImageCommand : IRequest<ServiceResponse<string>>
    {
    }
}