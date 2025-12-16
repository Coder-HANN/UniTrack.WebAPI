using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Ban.Command
{
    public class BanDeleteCommand : IRequest<ServiceResponse<string>>
    {
        public Guid BanId { get; set; }
    }
}
