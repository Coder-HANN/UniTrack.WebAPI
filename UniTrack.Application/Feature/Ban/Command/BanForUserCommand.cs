using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Ban.Command
{
    public class BanForUserCommand : IRequest<ServiceResponse<string>>
    {
        public Guid UserId { get; set; }
        public DateTime LastDate { get; set; }
        public string Description { get; set; }
    }
}
