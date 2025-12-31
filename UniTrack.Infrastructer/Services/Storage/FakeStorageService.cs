using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using UniTrack.Application.Abstraction.Services.Storage;
using UniTrack.Domain.Enums;

public class FakeStorageService : IStorageService
{
    private readonly string baseUrl;

    public FakeStorageService(IConfiguration configuration)
    {
        baseUrl = configuration["Storage:BaseUrl"];
    }

    public Task<string> UploadFileAsync(
        byte[] data,
        string fileName,
        string contentType = "image/png")
    {
        return Task.FromResult(
            $"{baseUrl}/storage/fake/qr/{fileName}"
        );
    }

    public Task<string> UploadFileAsync(
        byte[] data,
        string fileName,
        StorageFileType fileType,
        string contentType = "image/png")
    {
        var path = fileType switch
        {
            StorageFileType.EventImage => "events",
            StorageFileType.ClubImage => "clubs",
            StorageFileType.QrCode => "qr",
            _ => "others"
        };

        return Task.FromResult(
            $"{baseUrl}/storage/fake/{path}/{fileName}"
        );
    }

    public async Task DeleteFileAsync(string fileUrl)
    {
        // Fake storage olduğu için fiziksel silme yok
        // Ama loglamak çok faydalı olur
        Debug.WriteLine($"[FAKE STORAGE] File deleted: {fileUrl}");

        await Task.CompletedTask;
    }
}
