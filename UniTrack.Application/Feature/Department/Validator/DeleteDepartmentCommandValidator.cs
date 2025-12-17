using FluentValidation;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.Feature.Department.Command;

namespace UniTrack.Application.Feature.Department.Validator
{
    public class DeleteDepartmentCommandValidator: AbstractValidator<DeleteDepartmentCommand>
    {
        public DeleteDepartmentCommandValidator(IDepartmentRepository departmentRepository,ILocalizationService localizationService)
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage(ValidationKeys.FieldRequired);

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(ValidationKeys.FieldRequired);

            RuleFor(x => x)
                .MustAsync(async (command, _) =>
                    await departmentRepository.GetByIdAsync(command.Id, command.Name) != null)
                .WithMessage(ValidationKeys.DepartmentNotFound);
        }
    }
}
