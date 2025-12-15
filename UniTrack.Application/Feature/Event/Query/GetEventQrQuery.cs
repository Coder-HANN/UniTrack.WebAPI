using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetEventQrQuery : IRequest<ServiceResponse<string>>
    {
        public Guid EventId { get; set; }
        public GetEventQrQuery() { }
    }
}
