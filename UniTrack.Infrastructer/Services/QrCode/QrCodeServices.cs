using Microsoft.Extensions.Configuration;
using QRCoder;
using UniTrack.Application.Abstraction.Services.QrCode;

namespace UniTrack.Infrastructure.Services.QrCode
{
    public class QrCodeService : IQrCodeService
    {
        private readonly string _baseCheckInUrl;

        public QrCodeService(IConfiguration configuration)
        {
            _baseCheckInUrl = configuration["QrCode:BaseCheckInUrl"]
                ?? throw new InvalidOperationException("QrCode:BaseCheckInUrl configuration'da tanımlanmalıdır.");
        }

        public async Task<byte[]> GenerateQrCodeAsync(Guid checkInToken)
        {
            string payload = $"{_baseCheckInUrl}{checkInToken}";

            using (var generator = new QRCodeGenerator())
            {
                var qrCodeData = generator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
                using (var pngQrCode = new PngByteQRCode(qrCodeData))
                {
                    return pngQrCode.GetGraphic(pixelsPerModule: 5);
                }
            }
        }
    }
}