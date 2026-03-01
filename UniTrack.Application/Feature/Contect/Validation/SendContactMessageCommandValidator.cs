using FluentValidation;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;

namespace UniTrack.Application.Feature.Contect.Command
{
    public class SendContactMessageCommandValidator: AbstractValidator<SendContactMessageCommand>
    {
        public SendContactMessageCommandValidator(ILocalizationService localizationService)
        {
            
            RuleFor(x => x.Subject)
                .NotEmpty()
                .WithMessage(localizationService.Get(ValidationKeys.FieldRequired).Result)
                .MaximumLength(150)
                .WithMessage(localizationService.Get(ValidationKeys.MaxLengthExceeded).Result);

            RuleFor(x => x.Message)
                .NotEmpty()
                .WithMessage(localizationService.Get(ValidationKeys.FieldRequired).Result)
                .MaximumLength(1000)
                .WithMessage(localizationService.Get(ValidationKeys.MaxLengthExceeded).Result);
        }
    }
}
