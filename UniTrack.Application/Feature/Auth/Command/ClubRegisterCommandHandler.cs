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

        public ClubRegisterCommandHandler(
            IClubRepository clubRepository,
            IPasswordHasher<Domain.Entities.Club> passwordHasher,
            IVerificationCodeService codeService,
            ITransactionService transactionService,
            ILocalizationService localizationService)
        {
            this.clubRepository = clubRepository;
            this.passwordHasher = passwordHasher;
            this.codeService = codeService;
            this.transactionService = transactionService;
            this.localizationService = localizationService;
        }

        public async Task<ServiceResponse<ClubRegisterResponseDTO>> Handle(ClubRegisterCommand request,CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.PresidentEmail))
            {
                return ServiceResponse<ClubRegisterResponseDTO>.Fail(await localizationService.Get(ValidationKeys.PresidentEmailRequired));
            }

            var existingClub =await clubRepository.GetByEmailAndVerifyAsync(request.PresidentEmail);

            if (existingClub != null)
            {
                return ServiceResponse<ClubRegisterResponseDTO>.Fail(await localizationService.Get(ValidationKeys.ClubEmailAlreadyExists));
            }

            transactionService.Begin();

            try
            {
                var club = new Domain.Entities.Club
                {
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

                club.Password =passwordHasher.HashPassword(club, request.Password);

                await clubRepository.AddAsync(club);

                await codeService.GenerateAndSendCodeAsync(request.PresidentEmail,VerificationType.ClubRegistration);

                transactionService.Commit();

                return ServiceResponse<ClubRegisterResponseDTO>.Success(await localizationService.Get(ValidationKeys.VerificationCodeSent),
                    new ClubRegisterResponseDTO { ClubId = club.Id });
            }
            catch
            {
                transactionService.Rollback();

                return ServiceResponse<ClubRegisterResponseDTO>.Fail(await localizationService.Get(ValidationKeys.ClubRegisterFailed));
            }
        }
    }
}
