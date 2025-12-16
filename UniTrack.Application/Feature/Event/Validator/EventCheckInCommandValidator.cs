using FluentValidation;
using UniTrack.Application.Abstraction.Services.Localization;

namespace UniTrack.Application.Feature.Event.Command
{
    public class EventCheckInCommandValidator: AbstractValidator<EventCheckInCommand>
    {
        public EventCheckInCommandValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.EventCheckInId)
                .NotEmpty();
        }
    }
}
