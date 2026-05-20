using MediatR;
using Microsoft.AspNetCore.Identity;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Abstraction.Services.Transaction;
using UniTrack.Application.Abstraction.Services.VerificationCode;
using UniTrack.Application.Common;
using UniTrack.Application.Common.Constants;
using UniTrack.Application.DTOs.Auth;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Auth.Command
{
    public class ClubRegisterCommandHandler : IRequestHandler<ClubRegisterCommand, ServiceResponse<ClubRegisterResponseDTO>>
    {
        private readonly IClubRepository clubRepository;
        private readonly IPasswordHasher<Domain.Entities.Club> passwordHasher;
        private readonly IVerificationCodeService codeService;
        private readonly ITransactionService transactionService;
        private readonly ILocalizationService localizationService;
        private readonly IUserRepository userRepository;

        public ClubRegisterCommandHandler(
            IClubRepository clubRepository,
            IPasswordHasher<Domain.Entities.Club> passwordHasher,
            IVerificationCodeService codeService,
            ITransactionService transactionService,
            ILocalizationService localizationService,
            IUserRepository userRepository)
        {
            this.clubRepository = clubRepository;
            this.passwordHasher = passwordHasher;
            this.codeService = codeService;
            this.transactionService = transactionService;
            this.localizationService = localizationService;
            this.userRepository = userRepository;
        }

        public async Task<ServiceResponse<ClubRegisterResponseDTO>> Handle(ClubRegisterCommand request, CancellationToken cancellationToken)
        {
            // Validation - DB gerektirmeyen kontroller transaction dışında
            if (string.IsNullOrWhiteSpace(request.PresidentEmail))
            {
                return ServiceResponse<ClubRegisterResponseDTO>.Fail(
                    await localizationService.Get(ValidationKeys.PresidentEmailRequired));
            }

            transactionService.Begin();

            try
            {
                var existingUser = await userRepository.GetByEmailAsync(request.ContactEmail);
                if (existingUser != null)
                {
                    transactionService.Rollback();
                    return ServiceResponse<ClubRegisterResponseDTO>.Fail(
                        await localizationService.Get(ValidationKeys.EmailAlreadyUsed));
                }

                var existingClub = await clubRepository.GetByEmailAsync(request.ContactEmail);

                if (existingClub != null)
                {
                    // Zaten doğrulanmış kulüp
                    if (existingClub.IsVerified == true)
                    {
                        transactionService.Rollback();
                        return ServiceResponse<ClubRegisterResponseDTO>.Fail(
                            await localizationService.Get(ValidationKeys.ClubEmailAlreadyExists));
                    }

                    // Doğrulanmamış kulüp var → bilgileri güncelle, yeni kod gönder
                    existingClub.Name = request.ClubName;
                    existingClub.PresidentMail = request.PresidentEmail;
                    existingClub.ContectEmail = request.ContactEmail;
                    existingClub.President = request.PresidentName;
                    existingClub.UniversityId = request.UniversityId;
                    existingClub.CityId = request.CityId;
                    existingClub.Tag = request.Tag;
                    existingClub.Description = request.ClubName;
                    existingClub.Password = passwordHasher.HashPassword(existingClub, request.Password);

                    await clubRepository.UpdateAsync(existingClub);

                    transactionService.Commit();

                    // Commit sonrası yan etki
                    await codeService.GenerateAndSendCodeAsync(request.ContactEmail, VerificationType.ClubRegistration);

                    return ServiceResponse<ClubRegisterResponseDTO>.Success(
                        await localizationService.Get(ValidationKeys.VerificationCodeSent),
                        new ClubRegisterResponseDTO { ClubId = existingClub.Id });
                }

                // Yeni kulüp kaydı
                var club = new Domain.Entities.Club
                {
                    Id = Guid.NewGuid(),
                    Name = request.ClubName,
                    PresidentMail = request.PresidentEmail,
                    ContectEmail = request.ContactEmail,
                    President = request.PresidentName,
                    UniversityId = request.UniversityId,
                    CityId = request.CityId,
                    Tag = request.Tag,
                    Role = Role.Club,
                    Description = request.ClubName,
                    Follower = 0,
                    IsVerified = false
                };

                club.Password = passwordHasher.HashPassword(club, request.Password);

                await clubRepository.AddAsync(club);

                transactionService.Commit();

                // Commit sonrası yan etki
                await codeService.GenerateAndSendCodeAsync(request.ContactEmail, VerificationType.ClubRegistration);

                return ServiceResponse<ClubRegisterResponseDTO>.Success(
                    await localizationService.Get(ValidationKeys.VerificationCodeSent),
                    new ClubRegisterResponseDTO { ClubId = club.Id });
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
                    await codeService.GenerateAndSendCodeAsync(request.ContactEmail, VerificationType.ClubRegistration);

                    return ServiceResponse<ClubRegisterResponseDTO>.Success(
                        await localizationService.Get(ValidationKeys.VerificationCodeSent), null);
                }

                return ServiceResponse<ClubRegisterResponseDTO>.Fail(
                    await localizationService.Get(ValidationKeys.ClubRegisterFailed));
            }
        }
    }
}