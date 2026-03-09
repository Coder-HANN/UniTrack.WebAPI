using Google.Apis.Drive.v3;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Configuration;
using UniTrack.Application.Abstraction.Services.Sheets;

namespace UniTrack.Infrastructure.Services.Sheets;
public class GoogleSheetCreationService : IGoogleSheetCreationService
{
    private readonly SheetsService _sheetsService;
    private readonly DriveService _driveService; 
    private readonly string _rootFolderId;

    // Sheets'e yazılacak başlık satırları
    private readonly IList<object> _headerRow = new List<object>
    {
        "Katılım Tarihi", "Adı", "Soyadı", "Üniversite", "Bölüm",
        "Kaçıncı Sınıf", "Email", "Sponsor Katılımı"
    };

    // Constructor güncellendi: DriveService ve IConfiguration alıyor
    public GoogleSheetCreationService(SheetsService sheetsService, DriveService driveService, IConfiguration configuration)
    {
        _sheetsService = sheetsService;
        _driveService = driveService;

        // Klasör ID'sini Configuration'dan okuma
        var sheetsConfig = configuration.GetSection("GoogleSheets").Get<GoogleSheetsConfig>();

        if (sheetsConfig == null || string.IsNullOrEmpty(sheetsConfig.RootFolderId))
        {
            throw new InvalidOperationException("GoogleSheets:RootFolderId configuration'da tanımlanmalıdır.");
        }

        _rootFolderId = sheetsConfig.RootFolderId;
    }

    public async Task<string> CreateSheetAsync(string sheetTitle)
    {
        // 1. E-Tablo Oluşturma İsteği (Sheets API)
        var spreadsheet = new Spreadsheet()
        {
            Properties = new SpreadsheetProperties()
            {
                Title = $"Etkinlik Katılımcıları: {sheetTitle}"
            }
        };

        var request = _sheetsService.Spreadsheets.Create(spreadsheet);
        request.Fields = "spreadsheetId";
        var response = await request.ExecuteAsync();
        string sheetId = response.SpreadsheetId;

        if (string.IsNullOrEmpty(sheetId))
        {
            throw new Exception("Google Sheets API'den geçerli bir Spreadsheet ID alınamadı.");
        }

        // 2. Drive API ile Dosyayı Hedef Klasöre Taşıma (**YENİ VE KRİTİK KISIM**)
        var fileMetadata = new Google.Apis.Drive.v3.Data.File
        {
            // Yeni Parent ID'sini belirle
            Parents = new List<string> { _rootFolderId }
        };

        // Taşıma isteği: Mevcut (root) parent'ını kaldırıp, RootFolderId'yi ekle
        var updateRequest = _driveService.Files.Update(fileMetadata, sheetId);
        updateRequest.RemoveParents = "root";
        await updateRequest.ExecuteAsync();

        // 3. Başlık Satırını Ekleme İsteği (Sheets API)
        var valueRange = new ValueRange();
        valueRange.Values = new List<IList<object>> { _headerRow };

        var appendRequest = _sheetsService.Spreadsheets.Values.Append(valueRange, sheetId, "Sayfa1!A1");
        appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

        await appendRequest.ExecuteAsync();

        return sheetId;
    }
}