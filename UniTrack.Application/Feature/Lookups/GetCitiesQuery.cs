using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Lookup;

namespace UniTrack.Application.Feature.Lookups
{
    public class GetCitiesQuery : IRequest<ServiceResponse<IEnumerable<LookupServiceResponseDTO>>>
    {
        public GetCitiesQuery() { }
    }
}
