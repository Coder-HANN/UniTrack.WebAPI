using MediatR;
using UniTrack.Application.Abstraction.Repositories.Pagenation;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Report;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Report.Query
{
    public class GetReportForAdminQueryCommand : IRequest<ServiceResponse<IPagingExecutionResult<GetReportForAdminResponseDTO>>>
    {
        public int PageSize { get; set; }
        public int Page { get; set; }

        public GetReportForAdminQueryCommand(int pageSize, int page)
        {
            PageSize = pageSize;
            Page = page;
        }
        public GetReportForAdminQueryCommand() { }
    }
}
