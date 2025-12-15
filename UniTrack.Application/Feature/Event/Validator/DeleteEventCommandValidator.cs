using FluentValidation;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Feature.Event.Command
{
    public class DeleteEventCommandValidator: AbstractValidator<DeleteEventCommand>
    {
        public DeleteEventCommandValidator(ILocalizationService localization)
        {
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage(localization.Get(ValidationKeys.EventIdRequired));
        }
    }
}
