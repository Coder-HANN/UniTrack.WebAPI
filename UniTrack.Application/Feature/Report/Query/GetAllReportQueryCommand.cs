using MediatR;
using UniTrack.Application.Abstraction.Repositories.Pagenation;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Report;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Report.Query
{
    public class GetAllReportQueryCommand : IRequest<ServiceResponse<IPagingExecutionResult<GetAllReportResponseDTO>>>
    {
        public int PageSize { get; set; }
        public int Page { get; set; }

        public GetAllReportQueryCommand(int pageSize, int page)
        {
            PageSize = pageSize;
            Page = page;
        }
        public GetAllReportQueryCommand() { }
    }
}
