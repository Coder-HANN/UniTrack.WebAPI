// UniTrack.Application/DTOs/EventQuestion/EventQuestionResponseDTO.cs
using UniTrack.Application.DTOs.Event;

namespace UniTrack.Application.DTOs.EventQuestion
{
    public class EventQuestionResponseDTO
    {
        public Guid QuestionId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ProfileImageUrl { get; set; }
        public DateTime AskedAt { get; set; }
        public string QuestionText { get; set; }
        public EventQuestionAnswerDTO? Answer { get; set; }
    }

    
}