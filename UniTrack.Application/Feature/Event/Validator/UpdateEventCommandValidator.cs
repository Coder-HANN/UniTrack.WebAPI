using FluentValidation;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Feature.Event.Command
{
    public class UpdateEventCommandValidator: AbstractValidator<UpdateEventCommand>
    {
        public UpdateEventCommandValidator(ILocalizationService localization)
        {
            RuleFor(x => x.Title)
                .MinimumLength(3)
                .MaximumLength(150)
                .When(x => !string.IsNullOrWhiteSpace(x.Title))
                .WithMessage(localization.Get(ValidationKeys.UpdateTitleInvalid).Result);

            RuleFor(x => x.Description)
                .MinimumLength(10)
                .MaximumLength(1000)
                .When(x => !string.IsNullOrWhiteSpace(x.Description))
                .WithMessage(localization.Get(ValidationKeys.UpdateDescriptionInvalid).Result);

            RuleFor(x => x.StartDate)
                .Must(BeFutureDate)
                .When(x => x.StartDate != default)
                .WithMessage(localization.Get(ValidationKeys.UpdateStartDateInvalid).Result);

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate)
                .When(x => x.EndDate != default && x.StartDate != default)
                .WithMessage(localization.Get(ValidationKeys.UpdateEndDateInvalid).Result);

            RuleFor(x => x.Quota)
                .GreaterThan(0)
                .When(x => x.Quota > 0)
                .WithMessage(localization.Get(ValidationKeys.UpdateQuotaInvalid).Result);

            RuleFor(x => x.Location)
                .MinimumLength(3)
                .MaximumLength(200)
                .When(x => !string.IsNullOrWhiteSpace(x.Location))
                .WithMessage(localization.Get(ValidationKeys.UpdateLocationInvalid).Result);

            RuleFor(x => x.Clock)
                .Must(c => c != default)
                .When(x => x.Clock != default)
                .WithMessage(localization.Get(ValidationKeys.UpdateClockInvalid).Result);

            RuleFor(x => x.Status)
                .IsInEnum()
                .When(x => x.Status != default)
                .WithMessage(localization.Get(ValidationKeys.UpdateStatusInvalid).Result);

            RuleFor(x => x.Tag)
                .IsInEnum()
                .When(x => x.Tag != default)
                .WithMessage(localization.Get(ValidationKeys.UpdateTagInvalid).Result);
        }

        private bool BeFutureDate(DateTime date)
        {
            return date > DateTime.UtcNow;
        }
    }
}
