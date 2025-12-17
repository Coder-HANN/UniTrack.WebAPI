using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Department.Command
{
    public class CreateDepartmentCommandHandler: IRequestHandler<CreateDepartmentCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IDepartmentRepository departmentRepository;
        private readonly ILocalizationService localizationService;

        public CreateDepartmentCommandHandler(
            ICurrentUserServices currentUserServices,
            IDepartmentRepository departmentRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.departmentRepository = departmentRepository;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<string>> Handle(CreateDepartmentCommand request,CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            if (userId == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized)
                };
            }

            var role = currentUserServices.Role();
            if (role != Role.Admin)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = await localizationService.Get(ValidationKeys.NotAuthorized)
                };
            }

            var existingDepartment = await departmentRepository.GetDepartmentByNameAsync(request.Name);

            if (existingDepartment != null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = await localizationService.Get(ValidationKeys.DepartmentAlreadyExists)
                };
            }

            var department = new Domain.Entities.Department
            {
                Name = request.Name
            };

            await departmentRepository.AddAsync(department);

            return new ServiceResponse<string>
            {
                IsSuccess = true,
                Data = department.Name,
                Message = await localizationService.Get(ValidationKeys.DepartmentCreatedSuccessfully)
            };
        }
    }
}
