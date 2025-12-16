using FluentValidation;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.Auth.Command;

namespace UniTrack.Application.Feature.Auth.Validator
{
    public class UserRegisterCommandValidator : AbstractValidator<UserRegisterCommand>
    {
        public UserRegisterCommandValidator(ILocalizationService localizationService) 
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(localizationService.Get(ValidationKeys.FieldRequired).Result);

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage(localizationService.Get(ValidationKeys.FieldRequired).Result)
                .MinimumLength(6)
                .WithMessage(localizationService.Get(ValidationKeys.PasswordTooShort).Result);
        }
    }
}
