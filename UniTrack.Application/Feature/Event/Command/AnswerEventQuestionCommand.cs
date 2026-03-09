// UniTrack.Application/Feature/EventQuestion/Command/AnswerEventQuestionCommand.cs
using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.EventQuestion.Command
{
    public class AnswerEventQuestionCommand : IRequest<ServiceResponse<string>>
    {
        public Guid QuestionId { get; set; }
        public string AnswerText { get; set; }
    }
}