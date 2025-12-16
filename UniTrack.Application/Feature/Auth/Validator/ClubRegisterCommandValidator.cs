using FluentValidation;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Feature.Auth.Command
{
    public class ClubRegisterCommandValidator: AbstractValidator<ClubRegisterCommand>
    {
        public ClubRegisterCommandValidator(ILocalizationService localization)
        {
            RuleFor(x => x.ClubName)
                .NotEmpty()
                .WithMessage(localization.Get(ValidationKeys.FieldRequired).Result);

            RuleFor(x => x.PresidentName)
                .NotEmpty()
                .WithMessage(localization.Get(ValidationKeys.FieldRequired).Result);

            RuleFor(x => x.PresidentEmail)
                .NotEmpty()
                .WithMessage(localization.Get(ValidationKeys.FieldRequired).Result)
                .EmailAddress()
                .WithMessage(localization.Get(ValidationKeys.PresidentEmailInvalid).Result)
                .Must(BeEduMail)
                .When(x => !string.IsNullOrWhiteSpace(x.PresidentEmail))
                .WithMessage(localization.Get(ValidationKeys.PresidentEmailMustBeEdu).Result);

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage(localization.Get(ValidationKeys.FieldRequired).Result)
                .MinimumLength(6)
                .WithMessage(localization.Get(ValidationKeys.PasswordTooShort).Result);

            RuleFor(x => x.UniversityId)
                .NotEmpty()
                .WithMessage(localization.Get(ValidationKeys.FieldRequired).Result);

            RuleFor(x => x.CityId)
                .NotEmpty()
                .WithMessage(localization.Get(ValidationKeys.FieldRequired).Result);

            RuleFor(x => x.Tag)
                .IsInEnum()
                .WithMessage(localization.Get(ValidationKeys.ClubTagInvalid).Result);
        }

        private bool BeEduMail(string email)
        {
            return email.EndsWith(".edu.tr", StringComparison.OrdinalIgnoreCase);
        }
    }
}
