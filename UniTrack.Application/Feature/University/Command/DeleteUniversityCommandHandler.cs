using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.University.Command
{
    public class DeleteUniversityCommandHandler : IRequestHandler<DeleteUniversityCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IUniversityRepository universityRepository;
        public DeleteUniversityCommandHandler(
            ICurrentUserServices currentUserServices,
            IUniversityRepository universityRepository)
        {
            this.currentUserServices = currentUserServices;
            this.universityRepository = universityRepository;
        }
        public async Task<ServiceResponse<string>> Handle(DeleteUniversityCommand request, CancellationToken cancellationToken)
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
            var existingUniversity = await universityRepository.GetByIdAsync(request.Id);
            if (existingUniversity == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = "Üniversite bulunamadı."
                };
            }
            await universityRepository.DeleteAsync(existingUniversity);

            return new ServiceResponse<string>
            {
                IsSuccess = true,
                Message = "Üniversite başarıyla silindi."
            };
        }
    }
}
