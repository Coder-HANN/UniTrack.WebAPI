using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Lookup;

namespace UniTrack.Application.Feature.Lookups
{
    public class GetGenderQuery : IRequest<ServiceResponse<IEnumerable<LookupServiceResponseDTO>>>
    {
            public GetGenderQuery() { }
    }
}
