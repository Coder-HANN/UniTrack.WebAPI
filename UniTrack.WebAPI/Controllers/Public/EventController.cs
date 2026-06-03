using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniTrack.Application.Abstraction.Repositories.Pagenation;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Comment;
using UniTrack.Application.DTOs.Event;
using UniTrack.Application.Feature.Event.Command;
using UniTrack.Application.Feature.Event.Query;

namespace UniTrack.WebAPI.Controllers.Public
{
    [ApiController]
    [Route("api/public/[controller]")]
    [ApiExplorerSettings(GroupName = "Public")]
    public class EventController : ControllerBase
    {
        private readonly IMediator mediator;
        public EventController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("GetAllEventByClub")]
        public async Task<ServiceResponse<IPagingExecutionResult<GetClubEventQueryResponseDTO>>> GetAllEventByClub([FromQuery] GetClubEventQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpGet("GetAllFeatureEvents")]
        public async Task<ServiceResponse<IPagingExecutionResult<GetAllFeatureEventQueryResponseDTO>>> GetAllFeatureEvents([FromQuery] GetAllFeatureEventQuery query)
        {
            return await mediator.Send(query);
        }
        [HttpGet("GetAllPastEvents")]
        public async Task<ServiceResponse<IPagingExecutionResult<GetAllPastEventQueryResponseDTO>>> GetAllPastEvents([FromQuery] GetAllPastEventQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpGet("ClubIsEventCount")]
        public async Task<ServiceResponse<EventIsClubCountResponseDTO>> ClubIsEventCount([FromBody] EventIsClubCountCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpPost("UserJoinToEvent")]
        public async Task<ServiceResponse<UserJoinToEventResponseDTO>> UserJoinToEvent([FromBody] UserJoinToEventCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpPost("EventByLeft")]
        public async Task<ServiceResponse<string>> EventByLeft([FromBody] UserLeftToEventCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpGet("GetAllEventCount")]
        public async Task<ServiceResponse<long>> GetAllEventCount([FromQuery] GetAllEventCountQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpGet("GetAllFeatureEventCount")]
        public async Task<ServiceResponse<long>> GetAllFeatureEventCount([FromQuery] GetAllEventFeatureCountQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpGet("GetAllPastEventCount")]
        public async Task<ServiceResponse<long>> GetAllPastEventCount([FromQuery] GetAllEventPastCountQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpPost("EventCheckIn")]
        public async Task<ServiceResponse<string>> EventCheckIn([FromBody] EventCheckInCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpGet("GetJoinEventCountQuery")]
        public async Task<ServiceResponse<int>> GetJoinEventCountQuery([FromQuery] GetJoinEventCountQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpGet("GetEventDetail")]
        public async Task<ServiceResponse<GetEventDetailResponseDTO>> GetEventDetail([FromQuery] GetEventDetailCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpGet("GetAllJoinedEventForUser")]
        public async Task<ServiceResponse<IPagingExecutionResult<GetAllJoinedEventForUserQueryResponseDTO>>> GetAllJoinedEventForUser([FromQuery] GetAllJoinedEventForUserQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpGet("GetUpcomingEvents")]
        public async Task<ServiceResponse<List<UpcomingEventResponseDTO>>> GetUpcomingEvents([FromQuery] GetUpcomingEventsQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpGet("GetMonthlyParticipation")]
        public async Task<ServiceResponse<List<MonthlyParticipationResponseDTO>>> GetMonthlyParticipation([FromQuery] GetMonthlyParticipationQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpGet("GetCalendarEvents")]
        public async Task<ServiceResponse<List<CalendarEventResponseDTO>>> GetCalendarEvents([FromQuery] GetCalendarEventsQuery query)
        {
            return await mediator.Send(query);
        }
    }
}
