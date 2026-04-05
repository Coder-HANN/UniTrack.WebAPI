using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Configuration;
using UniTrack.Application.Abstraction.Services.Sheets;

namespace UniTrack.Infrastructure.Services.Sheets;

public class GoogleSheetCreationService : IGoogleSheetCreationService
{
    private readonly string _rootFolderId;
    private readonly string _applicationName = "Ogrencity";
    private readonly string[] _scopes = { DriveService.Scope.DriveFile, SheetsService.Scope.Spreadsheets };

    private readonly IList<object> _headerRow = new List<object>
    {
        "Katılım Tarihi", "Adı", "Soyadı", "Üniversite", "Bölüm",
        "Mezuniyet Tarihi", "Email", "Sponsor Katılımı"
    };

    public GoogleSheetCreationService(IConfiguration configuration)
    {
        var sheetsConfig = configuration.GetSection("GoogleSheets").Get<GoogleSheetsConfig>();
        _rootFolderId = sheetsConfig?.RootFolderId ?? throw new InvalidOperationException("RootFolderId eksik.");
    }

    // Bu metod kimlik doğrulamasını yapar
    private async Task<UserCredential> GetCredentialsAsync()
    {
        // Uygulamanın nerede çalıştığını anlıyoruz (Docker'da Production, Local'de Development olur)
        var isProduction = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";

        // Klasör yolunu ortamına göre belirliyoruz
        string authFolderPath = isProduction ? "/app/google_auth" : Directory.GetCurrentDirectory();

        string secretsPath = Path.Combine(authFolderPath, "service-account-key.json");
        string credPath = Path.Combine(authFolderPath, "token.json");

        using (var stream = new FileStream(secretsPath, FileMode.Open, FileAccess.Read))
        {
            return await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                _scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(credPath, true));
        }
    }

    public async Task<string> CreateSheetAsync(string eventId, string sheetTitle)
    {
        var credential = await GetCredentialsAsync();

        var driveService = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = _applicationName,
        });

        var sheetsService = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = _applicationName,
        });

        // İSİMLENDİRME BURADA QR MANTIĞIYLA EŞLEŞTİRİLDİ
        var fileMetadata = new Google.Apis.Drive.v3.Data.File()
        {
            Name = $"event-{eventId} - Etkinlik Katılımcıları: {sheetTitle}",
            MimeType = "application/vnd.google-apps.spreadsheet",
            Parents = new List<string> { _rootFolderId }
        };

        var createRequest = driveService.Files.Create(fileMetadata);
        createRequest.Fields = "id";
        var file = await createRequest.ExecuteAsync();
        string sheetId = file.Id;

        var valueRange = new ValueRange { Values = new List<IList<object>> { _headerRow } };
        var appendRequest = sheetsService.Spreadsheets.Values.Append(valueRange, sheetId, "A1");
        appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

        await appendRequest.ExecuteAsync();

        return sheetId;
    }
}