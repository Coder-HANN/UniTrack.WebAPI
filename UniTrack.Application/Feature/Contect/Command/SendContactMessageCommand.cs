using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Contect.Command
{
    public class SendContactMessageCommand : IRequest<ServiceResponse<string>>
    {
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
