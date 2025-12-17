using FluentValidation;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.Department.Command;

namespace UniTrack.Application.Feature.Department.Validator
{
    public class CreateDepartmentCommandValidator: AbstractValidator<CreateDepartmentCommand>
    {
        public CreateDepartmentCommandValidator(IDepartmentRepository departmentRepository,ILocalizationService localizationService)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(ValidationKeys.FieldRequired)

                .MinimumLength(2)
                .WithMessage(ValidationKeys.MinLength2)

                .MaximumLength(100)
                .WithMessage(ValidationKeys.MaxLength100)
                .MustAsync(async (name, _) =>
                    await departmentRepository.GetDepartmentByNameAsync(name) == null)
                .WithMessage(ValidationKeys.DepartmentAlreadyExists);
        }
    }
}
