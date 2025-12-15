using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniTrack.Application.Common;
using UniTrack.Application.Feature.Department.Command;
using UniTrack.Application.Feature.University.Command;

namespace UniTrack.WebAPI.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [ApiExplorerSettings(GroupName = "Admin")]
    public class UniversityController : ControllerBase
    {
        private readonly IMediator mediator;
        public UniversityController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("CreateUniversity")]
        public async Task<ServiceResponse<string>> CreateUniversity([FromBody] CreateUniversityCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpDelete("DeleteUniversity")]
        public async Task<ServiceResponse<string>> DeleteUniversity([FromBody] DeleteUniversityCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpPost("CreateDepartment")]
        public async Task<ServiceResponse<string>> CreateDepartment([FromBody] CreateDepartmentCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpDelete("DeleteDepartment")]
        public async Task<ServiceResponse<string>> DeleteDepartment([FromBody] DeleteDepartmentCommand command)
        {
            return await mediator.Send(command);
        }
    }
}
