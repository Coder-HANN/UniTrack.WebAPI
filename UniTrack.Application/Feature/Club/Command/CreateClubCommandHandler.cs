using AutoMapper;
using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Club.Command
{
    public class CreateClubCommandHandler : IRequestHandler<CreateClubCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IClubRepository clubServiceRepsoitory;
        private readonly IMapper mapper;
        public CreateClubCommandHandler(
            ICurrentUserServices currentUserServices,
            IClubRepository clubServiceRepsoitory,
            IMapper mapper)
        {
            this.currentUserServices = currentUserServices;
            this.clubServiceRepsoitory = clubServiceRepsoitory;
            this.mapper = mapper;
        }

        public async Task<ServiceResponse<string>> Handle(CreateClubCommand request, CancellationToken cancellationToken)
        {
            var userId = currentUserServices.CurrentUser();
            var clubId = currentUserServices.CurrentClub();
            if (userId == null && clubId == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Unauthorized"
                };
            }
            

            var role = currentUserServices.Role();

            if (role == null || role == Role.User)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Yetkisiz kullanıcı"
                };
            }

            var newClub = mapper.Map<Domain.Entities.Club>(request);

            await clubServiceRepsoitory.AddAsync(newClub);

            return new ServiceResponse<string>
            {
                IsSuccess = true,
                Data = null,
                Message = "Club created successfully"
            };
        }
    }
}
