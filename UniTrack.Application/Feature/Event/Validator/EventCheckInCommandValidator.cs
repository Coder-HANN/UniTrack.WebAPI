using FluentValidation;

namespace UniTrack.Application.Feature.Event.Command
{
    public class EventCheckInCommandValidator: AbstractValidator<EventCheckInCommand>
    {
        public EventCheckInCommandValidator()
        {
            RuleFor(x => x.EventCheckInId)
                .NotEmpty();
        }
    }
}
