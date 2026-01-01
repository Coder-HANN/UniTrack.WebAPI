using FluentValidation;
using UniTrack.Application.Feature.Notification.Command;

namespace UniTrack.Application.Feature.Notification.Validator
{

    public class SendNotificationCommandValidator: AbstractValidator<SendNotificationCommand>
    {
        public SendNotificationCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required.");

            RuleFor(x => x.Message)
                .NotEmpty()
                .WithMessage("Message is required.");

            RuleFor(x => x.Channels)
                .NotNull()
                .Must(c => c.Any())
                .WithMessage("At least one notification channel must be selected.");

            // Boş liste gönderilmesini engelle (null gönderilmeli)
            RuleFor(x => x.CityIds)
                .Must(BeNullOrNonEmpty)
                .WithMessage("CityIds cannot be an empty list. Use null instead.");

            RuleFor(x => x.UniversityIds)
                .Must(BeNullOrNonEmpty)
                .WithMessage("UniversityIds cannot be an empty list. Use null instead.");

            RuleFor(x => x.DepartmentIds)
                .Must(BeNullOrNonEmpty)
                .WithMessage("DepartmentIds cannot be an empty list. Use null instead.");

            RuleFor(x => x.ClubIds)
                .Must(BeNullOrNonEmpty)
                .WithMessage("ClubIds cannot be an empty list. Use null instead.");
        }

        private bool BeNullOrNonEmpty<T>(List<T>? list)
        {
            return list == null || list.Any();
        }
    }

}
