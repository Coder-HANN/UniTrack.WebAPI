using FluentValidation;

namespace UniTrack.Application.Feature.Notification.Command
{
    public class PushTargetNotificationForClubsCommandValidator: AbstractValidator<PushTargetNotificationForClubsCommand>
    {
        public PushTargetNotificationForClubsCommandValidator()
        {
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
