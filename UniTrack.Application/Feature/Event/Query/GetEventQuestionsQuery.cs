// UniTrack.Application/Feature/EventQuestion/Query/GetEventQuestionsQuery.cs
using MediatR;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.EventQuestion;

namespace UniTrack.Application.Feature.EventQuestion.Query
{
    public class GetEventQuestionsQuery : IRequest<ServiceResponse<List<EventQuestionResponseDTO>>>
    {
        public Guid EventId { get; set; }
    }
}