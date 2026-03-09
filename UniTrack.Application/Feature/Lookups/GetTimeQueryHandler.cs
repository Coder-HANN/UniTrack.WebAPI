using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Lookup;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Lookups
{
    public class GetTimeQueryHandler : IRequestHandler<GetTimeQuery, ServiceResponse<IEnumerable<LookupServiceResponseDTO>>>
    {
        public async Task<ServiceResponse<IEnumerable<LookupServiceResponseDTO>>> Handle(GetTimeQuery request, CancellationToken cancellationToken)
        {
            var tags = Enum.GetValues(typeof(Time))
               .Cast<Time>()
               .Select(t => new LookupServiceResponseDTO(
                   ((int)t).ToString(), // Enum'un sayısal değeri (0, 1, 2...)
                   t.ToString()         // Enum'un isim değeri (Teknoloji, Eğitim...)
               ));

            return ServiceResponse<IEnumerable<LookupServiceResponseDTO>>.Success(null, tags);
        }
    }
}
