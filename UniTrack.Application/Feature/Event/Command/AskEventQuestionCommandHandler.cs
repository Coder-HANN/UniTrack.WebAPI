// UniTrack.Application/Feature/EventQuestion/Command/AskEventQuestionCommandHandler.cs
using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Feature.EventQuestion.Command
{
    public class AskEventQuestionCommandHandler : IRequestHandler<AskEventQuestionCommand, ServiceResponse<string>>
    {
        private readonly IEventQuestionRepository questionRepository;
        private readonly IEventRepository eventRepository;
        private readonly ICurrentUserServices currentUser;
        private readonly ILocalizationService localization;

        public AskEventQuestionCommandHandler(
            IEventQuestionRepository questionRepository,
            IEventRepository eventRepository,
            ICurrentUserServices currentUser,
            ILocalizationService localization)
        {
            this.questionRepository = questionRepository;
            this.eventRepository = eventRepository;
            this.currentUser = currentUser;
            this.localization = localization;
        }

        public async Task<ServiceResponse<string>> Handle(AskEventQuestionCommand request, CancellationToken cancellationToken)
        {
            var userId = currentUser.CurrentUser();
            if (userId == null)
                return ServiceResponse<string>.Fail(await localization.Get(ValidationKeys.NotAuthorized));

            var eventEntity = await eventRepository.GetByIdAsync(request.EventId);
            if (eventEntity is null)
                return ServiceResponse<string>.Fail(await localization.Get(ValidationKeys.EventNotFound));

            var alreadyAsked = await questionRepository.HasUserAskedQuestionAsync(request.EventId, userId.Value);
            if (alreadyAsked)
                return ServiceResponse<string>.Fail("Bu etkinlik için zaten bir soru sordunuz.");

            var question = new Domain.Entities.EventQuestion
            {
                Id = Guid.NewGuid(),
                EventId = request.EventId,
                UserId = userId.Value,
                QuestionText = request.QuestionText,
                CreatedAt = DateTime.UtcNow
            };

            await questionRepository.AddAsync(question);

            return ServiceResponse<string>.Success(null, "Sorunuz başarıyla gönderildi.");
        }
    }
}