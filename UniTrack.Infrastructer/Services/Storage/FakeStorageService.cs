using UniTrack.Application.Abstraction.Services.Storage;
using System.Threading.Tasks;

namespace UniTrack.Infrastructure.Services.Storage
{
    public class FakeStorageService : IStorageService
    {
        public Task<string> UploadFileAsync(byte[] data, string fileName, string contentType = "image/png")
        {
            // TO DO :GERÇEK DEPOLAMA İŞLEMİ (Azure, AWS, Google Cloud) bu kısma gelecektir.

            // Şimdilik, benzersiz bir URL döndürüyoruz.
            string fakeUrl = $"https://unitrackstorage.mock/qr/events/{fileName}";

            return Task.FromResult(fakeUrl);
        }
    }
}