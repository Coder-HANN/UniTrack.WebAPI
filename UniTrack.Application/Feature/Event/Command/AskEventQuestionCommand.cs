// UniTrack.Application/Feature/EventQuestion/Command/AskEventQuestionCommand.cs
using MediatR;
using UniTrack.Application.Common;

namespace UniTrack.Application.Feature.EventQuestion.Command
{
    public class AskEventQuestionCommand : IRequest<ServiceResponse<string>>
    {
        public Guid EventId { get; set; }
        public string QuestionText { get; set; }
    }
}