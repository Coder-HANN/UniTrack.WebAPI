// UniTrack.Application/Feature/Club/Query/GetMonthlyFollowerCountQuery.cs
using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Club;

namespace UniTrack.Application.Feature.Club.Query
{
    public class GetMonthlyFollowerCountQuery : IRequest<ServiceResponse<List<MonthlyFollowerResponseDTO>>>
    {
        public GetMonthlyFollowerCountQuery() { }
    }
}