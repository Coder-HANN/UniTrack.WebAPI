using MediatR;
using UniTrack.Application.Abstraction.Repositories;
using UniTrack.Application.Abstraction.Services.VerificationCode;
using UniTrack.Application.Common;
using UniTrack.Application.Feature.VerificationCode.Command;
using UniTrack.Domain.Enums; 

public class VerifyClubCommandHandler : IRequestHandler<VerificationCommand, ServiceResponse<string>>
{
    private readonly IClubRepository _clubRepository;
    private readonly IVerificationCodeService _codeService;

    public VerifyClubCommandHandler(IClubRepository clubRepository, IVerificationCodeService codeService)
    {
        _clubRepository = clubRepository;
        _codeService = codeService;
    }

    public async Task<ServiceResponse<string>> Handle(VerificationCommand request, CancellationToken cancellationToken)
    {
        // 1. Kulübü bul
        var club = await _clubRepository.GetByEmailAsync(request.Email);
        if (club == null)
            return new ServiceResponse<string> { IsSuccess = false, Message = "Kulüp bulunamadı." };

        // 2. Kodu Generic Servis ile Kontrol Et (Tip: ClubRegistration)
        bool isValid = _codeService.ValidateCode(request.Email, request.VerificationCode, VerificationType.ClubRegistration);

        if (!isValid)
        {
            // İsteğe bağlı: Yanlış kod girilince kulübü silmek istiyorsanız:
            // await _clubRepository.DeleteAsync(club);
            return new ServiceResponse<string> { IsSuccess = false, Message = "Kod hatalı veya süresi dolmuş." };
        }

        // 3. Başarılı ise durumu güncelle
        club.IsVerified = true;
        await _clubRepository.UpdateAsync(club);

        // 4. Kodu temizle (Tekrar kullanılamasın)
        _codeService.RemoveCode(request.Email, VerificationType.ClubRegistration);

        return new ServiceResponse<string> { IsSuccess = true, Message = "Kulüp başarıyla doğrulandı." };
    }
}