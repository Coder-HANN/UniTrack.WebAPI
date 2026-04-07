using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Lookup;
using UniTrack.Application.Feature.Lookups;

namespace UniTrack.Application.Features.Lookups;

public class GetUniversitiesQueryHandler : IRequestHandler<GetUniversitiesQuery, ServiceResponse<IEnumerable<LookupServiceResponseDTO>>>
{
    private readonly IUniversityRepository _universityRepository;

    public GetUniversitiesQueryHandler(IUniversityRepository universityRepository)
    {
        _universityRepository = universityRepository;
    }

    public async Task<ServiceResponse<IEnumerable<LookupServiceResponseDTO>>> Handle(GetUniversitiesQuery request, CancellationToken cancellationToken)
    {
        var universities = await _universityRepository.GetAllAsync();

        var result = universities.OrderBy(u => u.Name).Select(u => new LookupServiceResponseDTO(
            u.Id.ToString(),
            u.Name
        ));

        return ServiceResponse<IEnumerable<LookupServiceResponseDTO>>.Success(null,result);
    }
}