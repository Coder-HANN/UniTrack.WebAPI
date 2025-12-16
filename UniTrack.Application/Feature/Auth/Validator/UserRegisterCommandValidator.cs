using FluentValidation;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.Auth.Command;

namespace UniTrack.Application.Feature.Auth.Validation
{
    public class UserRegisterCommandValidator : AbstractValidator<UserRegisterCommand>
    {
        public UserRegisterCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(ValidationKeys.FieldRequired);
                

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(ValidationKeys.FieldRequired)
                .MinimumLength(6).WithMessage(ValidationKeys.PasswordTooShort);

            RuleFor(x => x.Name)
                .NotEmpty().
                WithMessage(ValidationKeys.FieldRequired);

            RuleFor(x => x.Surname)
                .NotEmpty().
                WithMessage(ValidationKeys.FieldRequired);

            RuleFor(x => x.UniversityId)
                .NotEmpty()
                .WithMessage(ValidationKeys.FieldRequired);


            RuleFor(x => x.DepartmentId)
                .NotEmpty()
                .WithMessage(ValidationKeys.FieldRequired);


            RuleFor(x => x.CityId)
                .NotEmpty()
                .WithMessage(ValidationKeys.FieldRequired);

            RuleFor(x => x.BirthDate)
                .NotEmpty()
                .WithMessage(ValidationKeys.FieldRequired);

            RuleFor(x => x.Gender)
                .IsInEnum()
                .WithMessage(ValidationKeys.FieldRequired);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage(ValidationKeys.FieldRequired);
        }
    }
}
