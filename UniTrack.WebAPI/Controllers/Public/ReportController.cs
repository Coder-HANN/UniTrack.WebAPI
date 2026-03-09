using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniTrack.Application.Common;
using UniTrack.Application.Feature.Report.Command;

namespace UniTrack.WebAPI.Controllers.Public
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [ApiExplorerSettings(GroupName = "Public")]
    public class ReportController : Controller
    {
        private readonly IMediator mediator;

        public ReportController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("CreateClubReport")]
        public async Task<ServiceResponse<string>>GetClubReport([FromBody] CreateClubReportCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpPost("CreateEventReport")]
        public async Task<ServiceResponse<string>> GetEventReport([FromBody] CreateEventReportCommand command)
        {
            return await mediator.Send(command);
        }

    }
}
