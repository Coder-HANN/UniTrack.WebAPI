using MediatR;
using Microsoft.AspNetCore.Identity;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.CurrentUserServices;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.Transaction;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Auth;
using UniTrack.Domain.Entities;

namespace UniTrack.Application.Feature.Auth.Command
{
    public class AdminRegisterCommandHandler : IRequestHandler<AdminRegisterCommand, ServiceResponse<AdminRegisterResponseDTO>>
    {
        public readonly IUserRepository userRepository;
        public readonly IUserDetailRepository userDetailRepository;
        public readonly ICurrentUserServices currentUserServices;
        public readonly ILocalizationService localizationService;
        public readonly IPasswordHasher<User> passwordHasher;
        public readonly ITransactionService transactionService;

        public AdminRegisterCommandHandler(
            IUserRepository userRepository,
            ICurrentUserServices currentUserServices,
            IUserDetailRepository userDetailRepository,
            ILocalizationService localizationService,
            IPasswordHasher<User> passwordHasher,
            ITransactionService transactionService)
        {
            this.userRepository = userRepository;
            this.currentUserServices = currentUserServices;
            this.localizationService = localizationService;
            this.passwordHasher = passwordHasher;
            this.userDetailRepository = userDetailRepository;
            this.transactionService = transactionService;
        }

        public async Task<ServiceResponse<AdminRegisterResponseDTO>> Handle(AdminRegisterCommand command, CancellationToken cancellationToken) 
        {
            var role = currentUserServices.Role();
            if (role != Domain.Enums.Role.SuperAdmin)
            {
                return ServiceResponse<AdminRegisterResponseDTO>.Fail(await localizationService.Get(ValidationKeys.NotAuthorized));
            }

            var existingUser = await userRepository.GetAsync(u => u.Email == command.Email);
            if (existingUser != null)
            {
                return ServiceResponse<AdminRegisterResponseDTO>.Fail(await localizationService.Get(ValidationKeys.EmailAlreadyUsed));
            }

            transactionService.Begin();
            try
            {
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = command.Email,
                    Role = Domain.Enums.Role.Admin,
                };

                user.Password = passwordHasher.HashPassword(user, command.Password);

                await userRepository.AddAsync(user);

                var userdetail = new UserDetail
                {
                    UserId = user.Id,
                    Name = command.Name,
                    Surname = string.Empty,
                    UniverstiyId = command.UniversityId,
                };
                await userDetailRepository.AddAsync(userdetail);

                    transactionService.Commit();
            }
            catch (Exception ex)
            {
                transactionService.Rollback();

                return ServiceResponse<AdminRegisterResponseDTO>.Fail(await localizationService.Get(ValidationKeys.OperationFailed));
            }

            return new ServiceResponse<AdminRegisterResponseDTO>
            {
                IsSuccess = true,
                Data = new AdminRegisterResponseDTO
                {
                    Name = command.Name,
                    UniversityId = command.UniversityId
                },
                Message = await localizationService.Get(ValidationKeys.UserRegisterSuccess)
            };
        }
    }
}
