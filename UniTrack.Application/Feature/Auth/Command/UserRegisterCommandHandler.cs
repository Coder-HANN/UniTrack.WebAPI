using MediatR;
using Microsoft.AspNetCore.Identity;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.UserHub;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Auth;
using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Auth.Command
{
    public class UserRegisterCommandHandler : IRequestHandler<UserRegisterCommand, ServiceResponse<UserRegisterResponseDTO>>
    {
        private readonly IUserRepository userRepository;
        private readonly IUserDetailRepository userDetailRepository;
        private readonly IPasswordHasher<User> passwordHash;
        private readonly IUserRegisterCountService countService;
        private readonly ILocalizationService localizationService;

        public UserRegisterCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher<User> passwordHash,
            IUserDetailRepository userDetailRepository,
            IUserRegisterCountService countService,
            ILocalizationService localizationService)
        {
            this.userRepository = userRepository;
            this.passwordHash = passwordHash;
            this.userDetailRepository = userDetailRepository;
            this.countService = countService;
            this.localizationService = localizationService;
        }
        public async Task<ServiceResponse<UserRegisterResponseDTO>> Handle(UserRegisterCommand request, CancellationToken cancellationToken)
        {
            var mail = await userRepository.GetByEmailAsync(request.Email);
            if (mail != null)
            {
                return new ServiceResponse<UserRegisterResponseDTO>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = await localizationService.Get(ValidationKeys.EmailAlreadyExists)
                };
            }

            var hashedPassword = passwordHash.HashPassword(null,request.Password);

            var user = new User
            {
                Email = request.Email,
                Password = hashedPassword,
                Role = Role.User,
            };

            await userRepository.AddAsync(user);

            var userDetail = new UserDetail
            {
                UserId = user.Id,
                Name = request.Name,
                Surname = request.Surname,
                UniverstiyId = request.UniversityId,
                DepartmentId = request.DepartmentId,
                CityId = request.CityId
            };

            await userDetailRepository.AddAsync(userDetail);

            await countService.NotifyUserCountUpdatedAsync();


            return new ServiceResponse<UserRegisterResponseDTO>
            {
                IsSuccess = true,
                Data = null,
                Message = "Hesap oluşturuldu"
            };
        }
    }
}
