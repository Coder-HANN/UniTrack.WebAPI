using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Event;
using UniTrack.Application.DTOs.Profile;
using UniTrack.Application.Feature.Event.Command;
using UniTrack.Application.Feature.Event.Query;
using UniTrack.Application.Feature.EventImage.Command;

namespace UniTrack.WebAPI.Controllers.Club
{
    [ApiController]
    [Route("api/club/[controller]")]
    [ApiExplorerSettings(GroupName = "Club")]
    public class EventController : ControllerBase
    {
        private readonly IMediator mediator;
        public EventController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("CreateEvent")]
        public async Task<ServiceResponse<CreateEventResponseDTO>> CreateEvent([FromBody] CreateEventCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpPost("UpdateEvent")]
        public async Task<ServiceResponse<UpdateEventResponseDTO>> UpdateEvent([FromBody] UpdateEventCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpDelete("DeleteEvent")]
        public async Task<ServiceResponse<DeleteEventResponseDTO>> DeleteEvent([FromBody] DeleteEventCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpGet("GetClubEventJoin")]
        public async Task<ServiceResponse<List<GetClubEventJoinQueryResponseDTO>>> GetClubEventJoin([FromQuery]GetClubEventJoinQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpGet("GetQrKodForEvent")]
        public async Task<ServiceResponse<string>> GetQrKodForEvent([FromQuery] GetEventQrQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpGet("GetEventCheckInList")]
        public async Task<ServiceResponse<string>> GetEventCheckInList([FromQuery] GetEventSheetQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpGet("GetAllEventJoinerCount")]
        public async Task<ServiceResponse<long>> GetAllEventJoinerCount([FromQuery] GetAllClubEventJoinerCountQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpPost("UpdateEventImages")]
        public async Task<ServiceResponse<string>> UpdateEventImages([FromForm] UpdateEventImagesCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpPost("ChangeEventCoverImage")]
        public async Task<ServiceResponse<string>> ChangeEventCoverImage([FromForm] ChangeEventCoverImageCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpGet("GetClubEventReport")]
        public async Task<ServiceResponse<List<ClubEventReportResponseDTO>>> GetClubEventReport([FromQuery] GetClubEventReportQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpPost("UploadEventImage")]
        public async Task<ServiceResponse<UploadProfileImageResponseDTO>> UploadEventImage([FromForm] UploadEventImageCommand command)
        {
            return await mediator.Send(command);
        }
    }
}
