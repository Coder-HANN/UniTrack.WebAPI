using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Lookup;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Lookups
{
    public class GetCagetoryQueryHandler : IRequestHandler<GetCagetoryQuery, ServiceResponse<IEnumerable<LookupServiceResponseDTO>>>
    {
        public async Task<ServiceResponse<IEnumerable<LookupServiceResponseDTO>>> Handle(GetCagetoryQuery query, CancellationToken cancellationToken)
        {
            var tags = Enum.GetValues(typeof(Category))
               .Cast<Category>()
               .Select(t => new LookupServiceResponseDTO(
                   ((int)t).ToString(), // Enum'un sayısal değeri (0, 1, 2...)
                   t.ToString()         // Enum'un isim değeri (Yemek, Teknoloji...)
               ));

            return ServiceResponse<IEnumerable<LookupServiceResponseDTO>>.Success(null, tags);
        }
    }
}
