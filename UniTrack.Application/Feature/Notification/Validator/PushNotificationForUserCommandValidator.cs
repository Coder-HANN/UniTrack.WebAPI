using FluentValidation;

namespace UniTrack.Application.Feature.Notification.Command
{
    public class PushNotificationForUserCommandValidator: AbstractValidator<PushNotificationForUserCommand>
    {
        public PushNotificationForUserCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty();

            RuleFor(x => x.Message)
                .NotEmpty()
                .MaximumLength(500);

            RuleFor(x => x.Type)
                .IsInEnum();
        }
    }
}
