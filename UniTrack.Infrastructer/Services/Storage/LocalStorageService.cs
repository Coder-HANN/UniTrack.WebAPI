using Microsoft.Extensions.Configuration;
using UniTrack.Application.Abstraction.Services.Storage;
using UniTrack.Domain.Enums;

namespace UniTrack.Infrastructure.Services.Storage
{
    public class LocalStorageService : IStorageService
    {
        private readonly string _baseUrl;
        private readonly string _storagePath;

        public LocalStorageService(IConfiguration configuration)
        {
            _baseUrl = configuration["Storage:BaseUrl"]
                ?? throw new InvalidOperationException("Storage:BaseUrl tanımlanmamış.");
            _storagePath = configuration["Storage:PhysicalPath"]
                ?? throw new InvalidOperationException("Storage:PhysicalPath tanımlanmamış.");
        }

        public async Task<string> UploadFileAsync(byte[] data, string fileName, string contentType = "image/png")
        {
            return await SaveFileAsync(data, fileName, "others");
        }

        public async Task<string> UploadFileAsync(byte[] data, string fileName, StorageFileType fileType, string contentType = "image/png")
        {
            var folder = fileType switch
            {
                StorageFileType.EventImage => "events",
                StorageFileType.ClubImage => "clubs",
                StorageFileType.QrCode => "qr",
                StorageFileType.User => "users",
                StorageFileType.ESheets => "esheets",
                _ => "others"
            };

            return await SaveFileAsync(data, fileName, folder);
        }

        public async Task DeleteFileAsync(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl)) return;

            // URL'den fiziksel path'e çevir
            var relativePath = fileUrl.Replace(_baseUrl, "").TrimStart('/');
            var physicalPath = Path.Combine(_storagePath, relativePath.Replace('/', Path.DirectorySeparatorChar));

            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
            }

            await Task.CompletedTask;
        }

        private async Task<string> SaveFileAsync(byte[] data, string fileName, string folder)
        {
            // Fiziksel klasörü oluştur (yoksa)
            var folderPath = Path.Combine(_storagePath, folder);
            Directory.CreateDirectory(folderPath);

            // Dosyayı kaydet
            var filePath = Path.Combine(folderPath, fileName);
            await File.WriteAllBytesAsync(filePath, data);

            // Erişilebilir URL döndür
            return $"{_baseUrl}/storage/{folder}/{fileName}";
        }
    }
}