
namespace UniTrack.Application.Abstraction.Services.QrCode
{
    public interface IQrCodeService
    {
        /// <summary>
        /// Verilen Guid (CheckInToken) değerini kullanarak QR kod görüntüsünü oluşturur.
        /// </summary>
        /// <param name="checkInToken">QR kodunun içeriğini oluşturacak benzersiz token.</param>
        /// <returns>QR kodunun PNG formatında byte dizisi.</returns>
        Task<byte[]> GenerateQrCodeAsync(Guid checkInToken);
    }
}