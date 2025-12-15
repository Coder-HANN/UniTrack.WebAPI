using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniTrack.Application.Common;
using UniTrack.Application.Feature.City.Command;

namespace UniTrack.WebAPI.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [ApiExplorerSettings(GroupName = "Admin")]
    public class CityController : ControllerBase
    {
        private readonly IMediator mediator;
        public CityController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("CreateCity")]
        public async Task<ServiceResponse<string>> CreateCity([FromBody] CreateCityCommand command)
        {
            return await mediator.Send(command);
        }

    }
}