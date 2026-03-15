using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniTrack.Application.Abstraction.Repositories.Pagenation;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Club;
using UniTrack.Application.DTOs.ClubTeam;
using UniTrack.Application.Feature.Club.Command;
using UniTrack.Application.Feature.Club.Query;
using UniTrack.Application.Feature.ClubTeam.Query;
using UniTrack.Application.Feature.Notification.Command;

namespace UniTrack.WebAPI.Controllers.Public
{
    [ApiController]
    [Route("api/public/[controller]")]
    [ApiExplorerSettings(GroupName = "Public")]
    public class ClubController : ControllerBase
    {
        private readonly IMediator mediator;
        public ClubController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("GetAllClub")]
        public async Task<ServiceResponse<IPagingExecutionResult<GetAllClubQueryResponseDTO>>> GetAllClub([FromQuery] GetAllClubQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpGet("GetClubIsFollower")]
        public async Task<ServiceResponse<List<GetClubIsFollowerQueryResponseDTO>>> GetClubIsFollower([FromQuery] GetClubIsFollowerQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpGet("GetFollowClubById")]
        public async Task<ServiceResponse<IPagingExecutionResult<GetFollowClubQueryResponseDTO>>> GetFollowClubById([FromQuery] GetFollowClubQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpPost("ClubFollower")]
        public async Task<ServiceResponse<string>> ClubFollower([FromBody]  FollowClubCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpPost("ClubUnfollow")]
        public async Task<ServiceResponse<string>> ClubUnfollow([FromBody] UnfollowClubCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpPost("OpenNotificationForClub")]
        public async Task<ServiceResponse<string>> OpenNotificationForClub([FromBody] OpenNotificationForClubCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpPost("CloseNotificationForClub")]
        public async Task<ServiceResponse<string>> CloseNotificationForClub([FromBody] CloseNotificationForClubCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpGet("GetClubTeam")]
        public async Task<ServiceResponse<List<ClubTeamResponseDTO>>> GetClubTeam([FromQuery] ClubTeamQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpGet("GetAllClubCount")]
        public async Task<ServiceResponse<long>> GetAllClubCount([FromQuery] GetAllClubCountQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpGet("GetFollowClubCount")]
        public async Task<ServiceResponse<int>> GetFollowClubCount([FromQuery] GetFollowClubCountQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpGet("GetClubAllEventJoinerCount")]
        public async Task<ServiceResponse<int>> GetClubAllEventJoinerCount([FromQuery] GetClubAllEventJoinerCountQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpGet("GetClubDetail")]
        public async Task<ServiceResponse<GetClubDetailResponseDTO>> GetClubDetail([FromQuery] GetClubDetailQuery query)
        {
            return await mediator.Send(query);
        }
    }
}