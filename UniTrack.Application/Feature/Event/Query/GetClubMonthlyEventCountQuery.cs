using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Event;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetClubMonthlyEventCountQuery : IRequest<ServiceResponse<List<MonthlyParticipationResponseDTO>>>
    {
    }
}