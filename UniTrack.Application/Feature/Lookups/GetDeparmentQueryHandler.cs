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

            var result = cities
                .OrderBy(c => c.Name)
                .Select(c => new LookupServiceResponseDTO(
                    c.Id.ToString(),
                    c.Name
                ));

            return ServiceResponse<IEnumerable<LookupServiceResponseDTO>>.Success(null, result);
        }
    }
}
