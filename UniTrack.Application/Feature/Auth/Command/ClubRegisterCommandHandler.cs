using MediatR;
using Microsoft.AspNetCore.Identity;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.Transaction;
using UniTrack.Application.Abstraction.Services.VerificationCode;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Auth;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Feature.Auth.Command
{
    public class ClubRegisterCommandHandler : IRequestHandler<ClubRegisterCommand, ServiceResponse<ClubRegisterResponseDTO>>
    {
        private readonly IClubRepository _clubRepository;
        private readonly IPasswordHasher<Domain.Entities.Club> _passwordHasher;
        private readonly IVerificationCodeService _codeService;
        private readonly ITransactionService _transactionService;

        public ClubRegisterCommandHandler(
            IClubRepository clubRepository,
            IPasswordHasher<Domain.Entities.Club> passwordHasher,
            IVerificationCodeService codeService,
            ITransactionService transactionService)
        {
            _clubRepository = clubRepository;
            _passwordHasher = passwordHasher;
            _codeService = codeService;
            _transactionService = transactionService;
        }

        public async Task<ServiceResponse<ClubRegisterResponseDTO>> Handle(ClubRegisterCommand request, CancellationToken cancellationToken)
        {
           
            if (string.IsNullOrWhiteSpace(request.PresidentEmail))
            {
                return new ServiceResponse<ClubRegisterResponseDTO>
                {
                    IsSuccess = false,
                    Message = "Başkan e-posta adresi boş bırakılamaz."
                };
            }

            var existingClub = await _clubRepository.GetByEmailAsync(request.PresidentEmail);
            if (existingClub != null)
            {
                return new ServiceResponse<ClubRegisterResponseDTO>
                {
                    IsSuccess = false,
                    Message = "Bu e-posta adresi zaten kullanımda."
                };
            }

            // 2. Transaction Başlat
            _transactionService.Begin();
            try
            {
                // 3. Entity Oluşturma
                var club = new Domain.Entities.Club
                {
                    Name = request.ClubName,
                    PresidentMail = request.PresidentEmail,
                    ContectEmail = request.ContectEmail,
                    President = request.PresidentName,
                    UniversityId = request.UniversityId,
                    CityId = request.CityId,
                    Tag = request.Tag,
                    Role = Role.Club,
                    Description = request.ClubName + " Resmi Hesabı",
                    Follower = 0,
                    IsVerified = false
                };

                // 4. Şifreleme (Best Practice: 'null' yerine 'club' nesnesini veriyoruz)
                club.Password = _passwordHasher.HashPassword(club, request.Password);

                // 5. Veritabanına Ekleme
                await _clubRepository.AddAsync(club);

                // 6. Kod Üretme ve Mail Gönderme (Tek Satırda)
                // Handler artık mail içeriğiyle veya kodun nasıl üretildiğiyle ilgilenmez.
                // Sadece "Kulüp Kaydı için kod gönder" emrini verir.
                await _codeService.GenerateAndSendCodeAsync(request.PresidentEmail, VerificationType.ClubRegistration);

                // 7. İşlemi Tamamla
                _transactionService.Commit();

                return new ServiceResponse<ClubRegisterResponseDTO>
                {
                    IsSuccess = true,
                    Message = "Kayıt başarılı. Doğrulama kodu gönderildi.",
                    Data = new ClubRegisterResponseDTO { ClubId = club.Id }
                };
            }
            catch (Exception ex)
            {
                _transactionService.Rollback();
                // Loglama yapılabilir (örn: _logger.LogError(ex...))

                return new ServiceResponse<ClubRegisterResponseDTO>
                {
                    IsSuccess = false,
                    Message = "Bir hata oluştu: " + ex.Message,
                    Data = null
                };
            }
        }
    }
}