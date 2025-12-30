using FluentValidation;

namespace UniTrack.Application.Feature.Notification.Command
{
    public class PushNotificationForClubCommandValidator: AbstractValidator<PushNotificationForClubCommand>
    {
        public PushNotificationForClubCommandValidator()
        {
            RuleFor(x => x.ClubId)
                .NotEmpty();

            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(150);

            RuleFor(x => x.Message)
                .NotEmpty()
                .MaximumLength(500);

            RuleFor(x => x.Type)
                .IsInEnum();
        }
    }
}
