// UniTrack.Domain/Entities/EventQuestion.cs
namespace UniTrack.Domain.Entities
{
    public class EventQuestion
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        public string QuestionText { get; set; }
        public DateTime CreatedAt { get; set; }

        public Event Event { get; set; }
        public User User { get; set; }
        public EventQuestionAnswer? Answer { get; set; }
    }
}