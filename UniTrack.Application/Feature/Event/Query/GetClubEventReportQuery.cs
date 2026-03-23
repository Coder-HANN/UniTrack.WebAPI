using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Event;

namespace UniTrack.Application.Feature.Event.Query
{
    public class GetClubEventReportQuery : IRequest<ServiceResponse<List<ClubEventReportResponseDTO>>>
    {
        public GetClubEventReportQuery() { }
    }
}