using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.University.Command
{
    public class CreateUniversityCommandHandler : IRequestHandler<CreateUniversityCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IUniversityRepository universityRepository;
        public CreateUniversityCommandHandler(
            ICurrentUserServices currentUserServices,
            IUniversityRepository universityRepository)
        {
            this.currentUserServices = currentUserServices;
            this.universityRepository = universityRepository;
        }
        public async Task<ServiceResponse<string>> Handle(CreateUniversityCommand request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();

            if (userId == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Unauthorized"
                };
            }

            var role = currentUserServices.Role();

            if (role == null || role == Role.User || role == Role.Club)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Yetkisiz kullanıcı"
                };
            }

            await universityRepository.AddAsync(new Domain.Entities.University
            {
                Name = request.Name,
                CityId = request.CityId
            });

            return new ServiceResponse<string>
            {
                IsSuccess = true,
                Data = null,
                Message = "University created successfully"
            };
        }
    }
}