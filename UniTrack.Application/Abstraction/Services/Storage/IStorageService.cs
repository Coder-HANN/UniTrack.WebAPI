using UniTrack.Domain.Enums;

namespace UniTrack.Application.Abstraction.Services.Storage
{
    public interface IStorageService
    {
        Task DeleteFileAsync(string fileUrl);

        /// <summary>
        /// Görüntü verisini alır ve bulut depolama alanına yükler.
        /// </summary>
        /// <param name="data">Görüntünün byte dizisi.</param>
        /// <param name="fileName">Dosya adı (örn: event-GUID.png).</param>
        /// <returns>Erişilebilir görüntü URL'si.</returns>
        Task<string> UploadFileAsync(byte[] data, string fileName, string contentType = "image/png");
        Task<string> UploadFileAsync(byte[] data, string fileName, StorageFileType fileType, string contentType = "image/png");
    }
}