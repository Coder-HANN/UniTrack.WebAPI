// UniTrack.Application/Feature/EventQuestion/Query/GetEventQuestionsQueryHandler.cs
using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Event;
using UniTrack.Application.DTOs.EventQuestion;

namespace UniTrack.Application.Feature.EventQuestion.Query
{
    public class GetEventQuestionsQueryHandler : IRequestHandler<GetEventQuestionsQuery, ServiceResponse<List<EventQuestionResponseDTO>>>
    {
        private readonly IEventQuestionRepository questionRepository;
        private readonly ILocalizationService localization;

        public GetEventQuestionsQueryHandler(
            IEventQuestionRepository questionRepository,
            ILocalizationService localization)
        {
            this.questionRepository = questionRepository;
            this.localization = localization;
        }

        public async Task<ServiceResponse<List<EventQuestionResponseDTO>>> Handle(GetEventQuestionsQuery request, CancellationToken cancellationToken)
        {
            var questions = await questionRepository.GetByEventIdWithDetailsAsync(request.EventId);

            var result = questions.Select(q => new EventQuestionResponseDTO
            {
                QuestionId = q.Id,
                FirstName = q.User?.UserDetail?.Name,
                LastName = q.User?.UserDetail?.Surname,
                ProfileImageUrl = q.User?.UserDetail?.ProfileImageUrl,
                AskedAt = q.CreatedAt,
                QuestionText = q.QuestionText,
                Answer = q.Answer == null ? null : new EventQuestionAnswerDTO
                {
                    ClubName = q.Answer.Club?.Name,
                    AnsweredAt = q.Answer.AnsweredAt,
                    AnswerText = q.Answer.AnswerText
                }
            }).ToList();

            return ServiceResponse<List<EventQuestionResponseDTO>>.Success(null,result);
        }
    }
}