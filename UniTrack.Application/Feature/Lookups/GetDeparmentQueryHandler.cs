using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Lookup;

namespace UniTrack.Application.Feature.Lookups
{
    public class GetDeparmentQueryHandler : IRequestHandler<GetDeparmentQuery, ServiceResponse<IEnumerable<LookupServiceResponseDTO>>>
    {
        private readonly IDepartmentRepository departmentRepository;

        public GetDeparmentQueryHandler(IDepartmentRepository departmentRepository)
        {
            this.departmentRepository = departmentRepository;
        }

        public async Task<ServiceResponse<IEnumerable<LookupServiceResponseDTO>>> Handle(GetDeparmentQuery request, CancellationToken cancellationToken)
        {
            var cities = await departmentRepository.GetAllAsync();

            var result = cities.Select(c => new LookupServiceResponseDTO(
                c.Id.ToString(), // CityId int olsa bile DTO'da string gönderiyoruz (Kurumsal standart)
                c.Name
            ));

            return ServiceResponse<IEnumerable<LookupServiceResponseDTO>>.Success(null, result);
        }
    }
}
