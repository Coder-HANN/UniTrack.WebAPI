using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Department.Command
{
    public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IDepartmentRepository departmentRepository;
        public DeleteDepartmentCommandHandler(
            ICurrentUserServices currentUserServices,
            IDepartmentRepository departmentRepository)
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
                    Message = "Kullanıcı bulunamadı."
                };
            }
            var role = currentUserServices.Role();

            if (role != Role.Admin)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = "Bu işlemi gerçekleştirmek için yetkiniz yok."
                };
            }

            var existingDepartment = await departmentRepository.GetByIdAsync(request.Id,request.Name);

            if (existingDepartment == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = "Bölüm bulunamadı."
                };
            }

            await departmentRepository.DeleteAsync(existingDepartment);

            return new ServiceResponse<string>
            {
                IsSuccess = true,
                Message = "Bölüm başarıyla silindi."
            };
        }
    }
}
