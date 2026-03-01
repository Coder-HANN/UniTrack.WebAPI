using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Lookup;

namespace UniTrack.Application.Feature.Lookups
{
    public class GetTagsQuery : IRequest<ServiceResponse<IEnumerable<LookupServiceResponseDTO>>>
    {
        public GetTagsQuery() { }
    }
}
