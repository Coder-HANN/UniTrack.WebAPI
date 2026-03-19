using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Club;
using UniTrack.Application.DTOs.Event;
using UniTrack.Application.DTOs.Profile;
using UniTrack.Application.Feature.Club.Command;
using UniTrack.Application.Feature.Club.Query;
using UniTrack.Application.Feature.ClubTeam.Command;
using UniTrack.Application.Feature.Event.Query;
using UniTrack.Application.Feature.Profile.Command;
using UniTrack.Application.Feature.Profile.Query;

namespace UniTrack.WebAPI.Controllers.Club
{
    [ApiController]
    [Route("api/club/[controller]")]
    [ApiExplorerSettings(GroupName = "Club")]
    public class ClubController : ControllerBase
    {
        private readonly IMediator mediator;
        public ClubController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPut("ClubProfileUpdate")]
        public async Task<ServiceResponse<ClubProfileUpdateResponseDTO>> ClubProfileUpdate([FromBody] ClubProfileUpdateCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpGet("GetClubFollower")]
        public async Task<ServiceResponse<List<GetClubIsFollowerQueryResponseDTO>>> GetClubFollower([FromQuery] GetClubIsFollowerQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpPost("CreateClub")]
        public async Task<ServiceResponse<string>> CreateClub([FromBody] CreateClubCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpPost("CreateClubTeam")]
        public async Task<ServiceResponse<string>> CreateClubTeam([FromBody] CreateClubTeamCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpDelete("DeleteClubTeam")]
        public async Task<ServiceResponse<string>> DeleteClubTeam([FromBody] DeleteClubTeamCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpGet("GetClubFollowCount")]
        public async Task<ServiceResponse<int>> GetClubFollowCount([FromQuery] GetClubFollowerCountQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpGet("GetClubProfile")]
        public async Task<ServiceResponse<GetClubProfileResponseDTO>> GetClubProfile([FromQuery] GetClubProfileQuery query )
        {
            return await mediator.Send(query);
        }

        [HttpGet("GetClubUpcomingEvents")]
        public async Task<ServiceResponse<List<UpcomingEventResponseDTO>>> GetClubUpcomingEvents([FromQuery] GetClubUpcomingEventsQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpGet("GetClubEventCount")]
        public async Task<ServiceResponse<int>> GetClubEventCount([FromQuery] GetClubEventCountQuery query)
        {
            return await mediator.Send(query);
        }
        [HttpGet("GetClubMonthlyEventCount")]
        public async Task<ServiceResponse<List<MonthlyParticipationResponseDTO>>> GetClubMonthlyEventCount([FromQuery] GetClubMonthlyEventCountQuery query)
        {
            return await mediator.Send(query);
        }
    }
}
