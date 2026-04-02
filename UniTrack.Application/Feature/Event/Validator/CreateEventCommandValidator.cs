using FluentValidation;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Feature.Event.Command
{
    public class CreateEventCommandValidator
        : AbstractValidator<CreateEventCommand>
    {
        public CreateEventCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(150)
                .WithMessage(ValidationKeys.EventTitleRequired);

            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(1000)
                .WithMessage(ValidationKeys.EventDescriptionRequired);

            RuleFor(x => x.StartDate)
                .Must(BeFutureDate)
                .WithMessage(ValidationKeys.EventStartDateInvalid);

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate)
                .WithMessage(ValidationKeys.EventEndDateInvalid);

            // Clock kuralını kaldır, bunları ekle:
            RuleFor(x => x.StartTime)
                .Must(t => t != default)
                .WithMessage(ValidationKeys.EventClockRequired);

            RuleFor(x => x.EndTime)
                .Must((cmd, endTime) => endTime > cmd.StartTime)
                .WithMessage("Bitiş saati başlangıç saatinden sonra olmalıdır.");

            RuleFor(x => x.EventTag)
                .IsInEnum()
                .WithMessage(ValidationKeys.EventTagInvalid);

            RuleFor(x => x.Time)
                .IsInEnum()
                .WithMessage(ValidationKeys.EventTimeInvalied);

            RuleFor(x => x.Quota)
                .GreaterThan(0)
                .WithMessage(ValidationKeys.EventQuotaInvalid);

            RuleFor(x => x.Location)
                .NotEmpty()
                .MaximumLength(200)
                .WithMessage(ValidationKeys.EventLocationRequired);

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage(ValidationKeys.EventStatusInvalid);

            RuleFor(x => x.ClubId)
                .NotEmpty()
                .WithMessage(ValidationKeys.EventClubRequired);

            RuleFor(x => x.CityId)
                .GreaterThan(0)
                .WithMessage(ValidationKeys.EventCityRequired);

            RuleFor(x => x.UniversityId)
                .NotEmpty()
                .WithMessage(ValidationKeys.EventUniversityRequired);
        }

        private bool BeFutureDate(DateTime date)
        {
            return date > DateTime.UtcNow;
        }
    }
}
