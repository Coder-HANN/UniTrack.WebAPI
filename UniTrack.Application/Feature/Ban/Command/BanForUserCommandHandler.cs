using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Common;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Ban.Command
{
    public class BanForUserCommandHandler : IRequestHandler<BanForUserCommand, ServiceResponse<string>>
    {
        private readonly ICurrentUserServices currentUserServices;
        private readonly IUserRepository userRepository;
        private readonly IBanRepository banRepository;
        public BanForUserCommandHandler(
            ICurrentUserServices currentUserServices,
            IUserRepository userRepository,
            IBanRepository banRepository)
        {
            this.currentUserServices = currentUserServices;
            this.userRepository = userRepository;
            this.banRepository = banRepository;
        }

        public async Task<ServiceResponse<string>> Handle(BanForUserCommand request, CancellationToken cancellationToken)
        {
            var adminId = currentUserServices.CurrentUser();
            if (adminId == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = "Unauthorized",
                    Data = null
                };
            }
            var role = currentUserServices.Role();
            if (role == null || role == Role.Club || role == Role.User)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = "Yetkisiz kullanıcı"
                };
            }
            var user = await userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    Message = "User not found",
                    Data = null
                };
            }

            await banRepository.AddAsync(new Domain.Entities.Ban
            {
                UserId = request.UserId,
                LastDate = request.LastDate,
                IsBanned = true,
                Role = Role.User,
                Description = request.Description
            });

            return new ServiceResponse<string>
            {
                IsSuccess = true,
                Message = "User banned successfully",
                Data = null
            };

        }
    }
}
