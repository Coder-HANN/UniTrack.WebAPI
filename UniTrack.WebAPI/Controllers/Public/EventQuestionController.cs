// UniTrack.WebAPI/Controllers/Public/EventQuestionController.cs
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.EventQuestion;
using UniTrack.Application.Feature.EventQuestion.Command;
using UniTrack.Application.Feature.EventQuestion.Query;

namespace UniTrack.WebAPI.Controllers.Public
{
    [ApiController]
    [Route("api/public/[controller]")]
    [ApiExplorerSettings(GroupName = "Public")]
    public class EventQuestionController : ControllerBase
    {
        private readonly IMediator mediator;

        public EventQuestionController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("AskQuestion")]
        public async Task<ServiceResponse<string>> AskQuestion([FromBody] AskEventQuestionCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpGet("GetEventQuestions")]
        public async Task<ServiceResponse<List<EventQuestionResponseDTO>>> GetEventQuestions([FromQuery] GetEventQuestionsQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpPost("AnswerQuestion")]
        public async Task<ServiceResponse<string>> AnswerQuestion([FromBody] AnswerEventQuestionCommand command)
        {
            return await mediator.Send(command);
        }
    }
}