using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniTrack.Application.Common;
using UniTrack.Application.Feature.ActiveDetail.Query;

namespace UniTrack.WebAPI.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [ApiExplorerSettings(GroupName = "Admin")]
    public class ActiveUserOrClubController : ControllerBase
    {
        private readonly IMediator mediator;
        public ActiveUserOrClubController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("Get30DaysActiveUsers")]
        public async Task<ServiceResponse<long>> Get30DaysActiveUsers([FromQuery] GetActiveUsersFor30DaysQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpGet("Get90DaysActiveUsers")]
        public async Task<ServiceResponse<long>> Get90DaysActiveUsers([FromQuery] GetActiveUsersFor90DaysQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpGet("Get180DaysActiveUsers")]
        public async Task<ServiceResponse<long>> Get180DaysActiveUsers([FromQuery] GetActiveUsersFor180DaysQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpGet("Get360DaysActiveUsers")]
        public async Task<ServiceResponse<long>> Get360DaysActiveUsers([FromQuery] GetActiveUsersFor360DaysQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpGet("Get30DaysActiveClubs")]
        public async Task<ServiceResponse<long>> Get90DaysActiveClubs([FromQuery] GetActiveClubsFor30DaysQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpGet("Get90DaysActiveClubs")]
        public async Task<ServiceResponse<long>> Get90DaysActiveClubs([FromQuery] GetActiveClubsFor90DaysQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpGet("Get180DaysActiveClubs")]
        public async Task<ServiceResponse<long>> Get180DaysActiveClubs([FromQuery] GetActiveClubsFor180DaysQuery query)
        {
            return await mediator.Send(query);
        }

        [HttpGet("Get360DaysActiveClubs")]
        public async Task<ServiceResponse<long>> Get360DaysActiveClubs([FromQuery] GetActiveClubsFor360DaysQuery query)
        {
            return await mediator.Send(query);
        }
    }
}
