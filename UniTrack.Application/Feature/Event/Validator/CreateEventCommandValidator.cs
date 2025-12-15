using FluentValidation;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Feature.Event.Command
{
    public class CreateEventCommandValidator
        : AbstractValidator<CreateEventCommand>
    {
        public CreateEventCommandValidator(ILocalizationService localization)
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(150)
                .WithMessage(localization.Get(ValidationKeys.EventTitleRequired));

            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(1000)
                .WithMessage(localization.Get(ValidationKeys.EventDescriptionRequired));

            RuleFor(x => x.StartDate)
                .Must(BeFutureDate)
                .WithMessage(localization.Get(ValidationKeys.EventStartDateInvalid));

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate)
                .WithMessage(localization.Get(ValidationKeys.EventEndDateInvalid));

            RuleFor(x => x.Clock)
                .Must(c => c != default)
                .WithMessage(localization.Get(ValidationKeys.EventClockRequired));

            RuleFor(x => x.Tag)
                .IsInEnum()
                .WithMessage(localization.Get(ValidationKeys.EventTagInvalid));

            RuleFor(x => x.Quota)
                .GreaterThan(0)
                .WithMessage(localization.Get(ValidationKeys.EventQuotaInvalid));

            RuleFor(x => x.Location)
                .NotEmpty()
                .MaximumLength(200)
                .WithMessage(localization.Get(ValidationKeys.EventLocationRequired));

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage(localization.Get(ValidationKeys.EventStatusInvalid));

            RuleFor(x => x.ClubId)
                .NotEmpty()
                .WithMessage(localization.Get(ValidationKeys.EventClubRequired));

            RuleFor(x => x.CityId)
                .GreaterThan(0)
                .WithMessage(localization.Get(ValidationKeys.EventCityRequired));

            RuleFor(x => x.UniversityId)
                .NotEmpty()
                .WithMessage(localization.Get(ValidationKeys.EventUniversityRequired));
        }

        private bool BeFutureDate(DateTime date)
        {
            return date > DateTime.UtcNow;
        }
    }
}
