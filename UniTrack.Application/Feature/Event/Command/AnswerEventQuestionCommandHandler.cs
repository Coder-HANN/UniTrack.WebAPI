// UniTrack.Application/Feature/EventQuestion/Command/AnswerEventQuestionCommandHandler.cs
using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Feature.EventQuestion.Command
{
    public class AnswerEventQuestionCommandHandler : IRequestHandler<AnswerEventQuestionCommand, ServiceResponse<string>>
    {
        private readonly IEventQuestionRepository questionRepository;
        private readonly IEventQuestionAnswerRepository answerRepository;
        private readonly ICurrentUserServices currentUser;
        private readonly ILocalizationService localization;

        public AnswerEventQuestionCommandHandler(
            IEventQuestionRepository questionRepository,
            IEventQuestionAnswerRepository answerRepository,
            ICurrentUserServices currentUser,
            ILocalizationService localization)
        {
            this.questionRepository = questionRepository;
            this.answerRepository = answerRepository;
            this.currentUser = currentUser;
            this.localization = localization;
        }

        public async Task<ServiceResponse<string>> Handle(AnswerEventQuestionCommand request, CancellationToken cancellationToken)
        {
            var clubId = currentUser.CurrentClub();
            if (clubId == null)
                return ServiceResponse<string>.Fail(await localization.Get(ValidationKeys.NotAuthorized));

            var question = await questionRepository.GetByIdWithAnswerAsync(request.QuestionId);
            if (question is null)
                return ServiceResponse<string>.Fail("Soru bulunamadı.");

            // Sadece etkinliğin sahibi kulüp cevap verebilir
            if (question.Event?.ClubId != clubId.Value)
                return ServiceResponse<string>.Fail(await localization.Get(ValidationKeys.NotAuthorized));

            // Zaten cevaplanmışsa tekrar cevap veremesin
            if (question.Answer != null)
                return ServiceResponse<string>.Fail("Bu soru zaten cevaplanmış.");

            var answer = new Domain.Entities.EventQuestionAnswer
            {
                Id = Guid.NewGuid(),
                QuestionId = request.QuestionId,
                ClubId = clubId.Value,
                AnswerText = request.AnswerText,
                AnsweredAt = DateTime.UtcNow
            };

            await answerRepository.AddAsync(answer);

            return ServiceResponse<string>.Success(null, "Cevabınız başarıyla gönderildi.");
        }
    }
}