using QRCoder;
using UniTrack.Application.Abstraction.Services.QrCode;

namespace UniTrack.Infrastructure.Services.QrCode
{
    public class QrCodeService : IQrCodeService
    {
        // Örnek: Kodu okuyacak uygulamanın yönlendirileceği baz URL
        // (Bu URL'i appsettings.json'dan çekmek daha iyi olabilir, şimdilik sabit tuttuk.)
        private const string BaseCheckInUrl = "https://unitrack.app/checkin/";

        public async Task<byte[]> GenerateQrCodeAsync(Guid checkInToken)
        {
            // 1. QR kodunun içereceği payload (yönlendirilecek URL)
            string payload = $"{BaseCheckInUrl}{checkInToken}";

            using (var generator = new QRCodeGenerator())
            {
                // Hata Düzeltme Seviyesi (ECCLevel): Yüksek seviye, daha az hata demektir.
                var qrCodeData = generator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);

                // 2. Görüntüyü PNG formatında oluşturma
                using (var pngQrCode = new PngByteQRCode(qrCodeData))
                {
                    // Modül Boyutunu (Pixels per module) ayarlayabiliriz. 5 idealdir.
                    byte[] qrCodeBytes = pngQrCode.GetGraphic(pixelsPerModule: 5);
                    return qrCodeBytes;
                }
            }
        }
    }
}