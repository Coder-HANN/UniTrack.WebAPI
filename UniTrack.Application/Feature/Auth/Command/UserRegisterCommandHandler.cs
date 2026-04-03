using MediatR;
using Microsoft.AspNetCore.Identity;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.Transaction;
using UniTrack.Application.Abstraction.Services.UserHub;
using UniTrack.Application.Abstraction.Services.VerificationCode;
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
        private readonly IVerificationCodeService _verificationCodeService;

        public UserRegisterCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher<User> passwordHash,
            IUserDetailRepository userDetailRepository,
            IUserRegisterCountService countService,
            ITransactionService transactionService,
            ILocalizationService localizationService,
            IVerificationCodeService verificationCodeService)
        {
            this.userRepository = userRepository;
            this.passwordHash = passwordHash;
            this.userDetailRepository = userDetailRepository;
            this.countService = countService;
            this.localizationService = localizationService;
            this.transactionService = transactionService;
            this._verificationCodeService = verificationCodeService;
        }

        public async Task<ServiceResponse<UserRegisterResponseDTO>> Handle(UserRegisterCommand request, CancellationToken cancellationToken)
        {
            transactionService.Begin();

            try
            {
                var existingUser = await userRepository.GetByEmailAsync(request.Email);

                if (existingUser != null)
                {
                    // Zaten doğrulanmış kullanıcı
                    if (existingUser.IsVerified == true)
                    {
                        transactionService.Rollback();
                        return new ServiceResponse<UserRegisterResponseDTO>
                        {
                            IsSuccess = false,
                            Data = null,
                            Message = await localizationService.Get(ValidationKeys.UserEmailAlreadyExists)
                        };
                    }

                    // Doğrulanmamış kullanıcı var → bilgileri güncelle, yeni kod gönder
                    existingUser.Password = passwordHash.HashPassword(null, request.Password);
                    await userRepository.UpdateAsync(existingUser);

                    var existingDetail = await userDetailRepository.GetByUserIdAsync(existingUser.Id);
                    if (existingDetail != null)
                    {
                        existingDetail.Name = request.Name;
                        existingDetail.Surname = request.Surname;
                        existingDetail.UniverstiyId = request.UniversityId;
                        existingDetail.DepartmentId = request.DepartmentId;
                        existingDetail.CityId = request.CityId;
                        existingDetail.Gender = request.Gender;
                        existingDetail.BirthDate = request.BirthDate;
                        existingDetail.Graduaiton_Date = request.Graduaiton_Date;
                        await userDetailRepository.UpdateAsync(existingDetail);
                    }

                    transactionService.Commit();

                    // Commit sonrası yan etkiler
                    await _verificationCodeService.GenerateAndSendCodeAsync(request.Email, VerificationType.UserRegistration);

                    return new ServiceResponse<UserRegisterResponseDTO>
                    {
                        IsSuccess = true,
                        Data = null,
                        Message = await localizationService.Get(ValidationKeys.UserRegisterSuccess)
                    };
                }

                // Yeni kullanıcı kaydı
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

                transactionService.Commit();

                // Commit sonrası yan etkiler
                await countService.NotifyUserCountUpdatedAsync();
                await _verificationCodeService.GenerateAndSendCodeAsync(request.Email, VerificationType.UserRegistration);

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

                // Race condition: unique constraint ihlali
                bool isUniqueViolation =
                    ex.InnerException?.Message.Contains("unique", StringComparison.OrdinalIgnoreCase) == true ||
                    ex.InnerException?.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) == true;

                if (isUniqueViolation)
                {
                    await _verificationCodeService.GenerateAndSendCodeAsync(request.Email, VerificationType.UserRegistration);

                    return new ServiceResponse<UserRegisterResponseDTO>
                    {
                        IsSuccess = true,
                        Data = null,
                        Message = await localizationService.Get(ValidationKeys.UserRegisterSuccess)
                    };
                }

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