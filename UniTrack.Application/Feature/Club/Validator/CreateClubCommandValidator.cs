using FluentValidation;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.Club.Command;

namespace UniTrack.Application.Feature.Club.Validator
{
    public class CreateClubCommandValidator : AbstractValidator<CreateClubCommand>
    {
        public CreateClubCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(ValidationKeys.FieldRequired);

            RuleFor(x => x.UniversityId)
                .NotEmpty()
                .WithMessage(ValidationKeys.FieldRequired);

            RuleFor(x => x.President)
                .NotEmpty()
                .WithMessage(ValidationKeys.FieldRequired);

            RuleFor(x => x.ContectEmail)
                .NotEmpty()
                .WithMessage(ValidationKeys.FieldRequired)
                .EmailAddress()
                .WithMessage(ValidationKeys.InvalidEmail);

            RuleFor(x => x.ClubCreatedDate)
                .NotEmpty()
                .WithMessage(ValidationKeys.FieldRequired)
                .Must(date => date <= DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage(ValidationKeys.InvalidDate);

            RuleFor(x => x.Tag)
                .IsInEnum()
                .WithMessage(ValidationKeys.FieldRequired);
        }
    }
}
