using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniTrack.Application.Abstraction.Repositories.Pagenation;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Report;
using UniTrack.Application.Feature.Report.Command;
using UniTrack.Application.Feature.Report.Query;

namespace UniTrack.WebAPI.Controllers.Admin
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [ApiExplorerSettings(GroupName = "Admin")]
    public class ReportController : Controller
    {
        private readonly IMediator mediator;

        public ReportController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("GetAllReport")]
        public async Task<ServiceResponse<IPagingExecutionResult<GetAllReportResponseDTO>>>GetAllReport([FromQuery] GetAllReportQueryCommand query)
        {
            return await mediator.Send(query);
        }

        [HttpPost("MarkReportAsReviewed")]
        public async Task<ServiceResponse<string>> MarkReportAsReviewed([FromBody] MarkReportAsReviewedCommand command)
        {
            return await mediator.Send(command);
        }
    }
}
