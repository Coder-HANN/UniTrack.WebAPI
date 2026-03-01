using MediatR;
using Microsoft.AspNetCore.Identity;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.Transaction;
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
        private readonly ITransactionService transactionService;

        public UserRegisterCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher<User> passwordHash,
            IUserDetailRepository userDetailRepository,
            IUserRegisterCountService countService,
            ITransactionService transactionService,
            ILocalizationService localizationService)
        {
            this.userRepository = userRepository;
            this.passwordHash = passwordHash;
            this.userDetailRepository = userDetailRepository;
            this.countService = countService;
            this.localizationService = localizationService;
            this.transactionService = transactionService;
        }
        public async Task<ServiceResponse<UserRegisterResponseDTO>> Handle(UserRegisterCommand request, CancellationToken cancellationToken)
        {
            transactionService.Begin();

            
                var mail = await userRepository.GetByEmailAsync(request.Email);
                if (mail != null)
                {
                    return new ServiceResponse<UserRegisterResponseDTO>
                    {
                        IsSuccess = false,
                        Data = null,
                        Message = await localizationService.Get(ValidationKeys.UserEmailAlreadyExists)
                    };
                }
            try
            {
                var hashedPassword = passwordHash.HashPassword(null, request.Password);

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = request.Email,
                    Password = hashedPassword,
                    Role = Role.User,
                };

                await userRepository.AddAsync(user);

                var userDetail = new UserDetail
                {
                    Id = Guid.NewGuid(),
                    User = user,
                    Name = request.Name,
                    Surname = request.Surname,
                    UniverstiyId = request.UniversityId,
                    DepartmentId = request.DepartmentId,
                    CityId = request.CityId,
                    Gender = request.Gender,
                    BirthDate = request.BirthDate,
                    Graduaiton_Date = request.Graduaiton_Date,
                    Language = "tr"
                };

                await userDetailRepository.AddAsync(userDetail);

                await countService.NotifyUserCountUpdatedAsync();

                transactionService.Commit();

                return new ServiceResponse<UserRegisterResponseDTO>
                {
                    IsSuccess = true,
                    Data = null,
                    Message = await localizationService.Get(ValidationKeys.UserRegisterSuccess)
                };
            }
            catch (Exception ex)
            {
                transactionService.Rollback();
                string errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return new ServiceResponse<UserRegisterResponseDTO>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = await localizationService.Get("İşlem başarısız")
                };
                
            }
        }
    }
}
