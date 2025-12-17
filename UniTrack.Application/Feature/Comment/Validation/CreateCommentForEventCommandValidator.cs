using FluentValidation;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.Comment.Command;

namespace UniTrack.Application.Feature.Comment.Validator
{
    public class CreateCommentForEventCommandValidator: AbstractValidator<CreateCommentForEventCommand>
    {
        public CreateCommentForEventCommandValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage(ValidationKeys.FieldRequired);

            RuleFor(x => x.Point)
                .InclusiveBetween(1, 5)
                .WithMessage(ValidationKeys.PointInvalid);

            RuleFor(x => x.Descripiton)
                .NotEmpty()
                .WithMessage(ValidationKeys.FieldRequired)
                .MaximumLength(500);
        }
    }
}
