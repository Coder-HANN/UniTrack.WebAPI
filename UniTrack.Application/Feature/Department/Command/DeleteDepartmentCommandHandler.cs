using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Department.Command
{
    public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IDepartmentRepository departmentRepository;
        private readonly ILocalizationService localizationService;
        public DeleteDepartmentCommandHandler(
            ICurrentUserServices currentUserServices,
            IDepartmentRepository departmentRepository,
            ILocalizationService localizationService)
        {
            this.currentUserServices = currentUserServices;
            this.departmentRepository = departmentRepository;
        }

        public async Task<ServiceResponse<string>> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
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

            var existingDepartment = await departmentRepository.GetByIdAsync(request.Id,request.Name);

            if (existingDepartment == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = await localizationService.Get(ValidationKeys.DepartmentNotFound)
                };
            }

            await departmentRepository.DeleteAsync(existingDepartment);

            return new ServiceResponse<string>
            {
                IsSuccess = true,
                Message = await localizationService.Get(ValidationKeys.DepartmentDeletedSuccessfully)
            };
        }
    }
}
