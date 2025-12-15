using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.City.Command
{
    public class CreateCityCommandHandler : IRequestHandler<CreateCityCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly ICityRepository cityRepository;
        public CreateCityCommandHandler(
            ICurrentUserServices currentUserServices,
            ICityRepository cityRepository)
        {
            this.currentUserServices = currentUserServices;
            this.cityRepository = cityRepository;
        }
        public async Task<ServiceResponse<string>> Handle(CreateCityCommand request, CancellationToken cancellationToken)
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

            await cityRepository.AddAsync(new Domain.Entities.City
            {
                Name = request.Name,
            });

            return new ServiceResponse<string>
            {
                IsSuccess = true,
                Data = null,
                Message = "City created successfully"
            };
        }
    }
}
