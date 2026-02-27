using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Lookup;

namespace UniTrack.Application.Feature.Lookups
{
    public class GetCitiesQueryHandler : IRequestHandler<GetCitiesQuery, ServiceResponse<IEnumerable<LookupServiceResponseDTO>>>
    {
        private readonly ICityRepository _cityRepository;

        public GetCitiesQueryHandler(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }

        public async Task<ServiceResponse<IEnumerable<LookupServiceResponseDTO>>> Handle(GetCitiesQuery request, CancellationToken cancellationToken)
        {
            var cities = await _cityRepository.GetAllAsync();

            var result = cities.Select(c => new LookupServiceResponseDTO(
                c.Id.ToString(), // CityId int olsa bile DTO'da string gönderiyoruz (Kurumsal standart)
                c.Name
            ));

            return ServiceResponse<IEnumerable<LookupServiceResponseDTO>>.Success(null,result);
        }
    }
}
