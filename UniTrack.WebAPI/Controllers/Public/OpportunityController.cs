using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniTrack.Application.Abstraction.Repositories.Pagenation;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Opportunity;
using UniTrack.Application.DTOs.Profile;
using UniTrack.Application.Feature.Opportunity.Command;
using UniTrack.Application.Feature.Opportunity.Query;
using UniTrack.Application.Feature.OpportunityImage.Command;

namespace UniTrack.WebAPI.Controllers.Public
{
    [ApiController]
    [Route("api/public/[action]")]
    [ApiExplorerSettings(GroupName = "Public")]
    public class OpportunityController : Controller
    {
        private readonly IMediator mediator;

        public OpportunityController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("GetAllOpportunities")]
        public async Task<ServiceResponse<IPagingExecutionResult<GetAllOpportunityResponseDTO>>> GetAllOpportunities([FromQuery] GetAllOpportunityQuery query)
        {
            return await mediator.Send(query);
        }

        [Authorize]
        [HttpPost("CreateOpportunity")]
        public async Task<ServiceResponse<string>> CreateOpportunity([FromBody] CreateOpportunityCommand command)
        {
            return await mediator.Send(command);
        }

        [Authorize]
        [HttpPost("ViewedCodeForOpportunity")]
        public async Task<ServiceResponse<ViewedCodeForOpportunityResponseDTO>> ViewedCodeForOpportunity([FromBody] ViewedCodeForOpportunityCommand command)
        {
            return await mediator.Send(command);
        }

        [Authorize]
        [HttpPut("UpdateOpportunity")]
        public async Task<ServiceResponse<string>> UpdateOpportunity([FromBody] UpdateOpportunityCommand command)
        {
            return await mediator.Send(command);
        }

        [Authorize]
        [HttpDelete("DeleteOpportunity")]
        public async Task<ServiceResponse<string>> DeleteOpportunity([FromBody] DeleteOpportunityCommand command)
        {
            return await mediator.Send(command);
        }

        [Authorize]
        [HttpPost("UploadOpportunityImage")]
        public async Task<ServiceResponse<UploadProfileImageResponseDTO>> UploadOpportunityImage([FromForm] UploadOpportunityImageCommand command)
        {
            return await mediator.Send(command);
        }

        [Authorize]
        [HttpGet("GetUniversityOpportunities")]
        public async Task<ServiceResponse<IPagingExecutionResult<GetAllOpportunityResponseDTO>>> GetUniversityOpportunities([FromQuery] GetUniversityOpportunityQuery query)
        {
            return await mediator.Send(query);
        }
    }
}
