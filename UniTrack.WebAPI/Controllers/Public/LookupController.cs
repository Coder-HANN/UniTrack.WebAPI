using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Lookup;
using UniTrack.Application.Feature.Lookups;

namespace UniTrack.WebAPI.Controllers.Public
{
    [ApiController]
    [Route("api/public/[controller]")]
    public class LookupController : ControllerBase
    {
        private readonly IMediator mediator;
        public LookupController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("Universities")]
        public async Task<ServiceResponse<IEnumerable<LookupServiceResponseDTO>>> Universities([FromQuery] GetUniversitiesQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpGet("Cities")]
        public async Task<ServiceResponse<IEnumerable<LookupServiceResponseDTO>>> Cities([FromQuery] GetCitiesQuery query)
        {
            return await mediator.Send(query);
        }
    }
}
