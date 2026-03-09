// UniTrack.Domain/Entities/EventQuestionAnswer.cs
namespace UniTrack.Domain.Entities
{
    public class EventQuestionAnswer
    {
        public Guid Id { get; set; }
        public Guid QuestionId { get; set; }
        public Guid ClubId { get; set; }
        public string AnswerText { get; set; }
        public DateTime AnsweredAt { get; set; }

        public EventQuestion Question { get; set; }
        public Club Club { get; set; }
    }
}