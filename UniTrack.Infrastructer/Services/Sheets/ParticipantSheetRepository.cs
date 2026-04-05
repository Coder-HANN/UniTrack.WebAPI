using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using UniTrack.Application.Abstraction.Services.Sheets;

namespace UniTrack.Infrastructure.Services.Sheets;

public class ParticipantSheetRepository : IParticipantSheetRepository
{
    private readonly SheetsService _sheetsService;

    // Sütun indeksleri — tabloya göre sabit
    private const int ColJoinDate = 0; // A
    private const int ColName = 1; // B
    private const int ColSurname = 2; // C
    private const int ColUniversity = 3; // D
    private const int ColDepartment = 4; // E
    private const int ColGrade = 5; // F — Kaçıncı Sınıf
    private const int ColEmail = 6; // G
    private const int ColSponsor = 7; // H
    private const int ColCheckIn = 8; // I

    public ParticipantSheetRepository(GoogleCredential credential)
    {
        _sheetsService = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "UniTrack CampusConnect App",
        });
    }

    /// <summary>
    /// Spreadsheet'in ilk sayfasının adını döner.
    /// Dile bağımsız çalışır (Sheet1 / Sayfa1 / Tabelle1 fark etmez).
    /// </summary>
    private async Task<string> GetFirstSheetTitleAsync(string sheetId)
    {
        var spreadsheet = await _sheetsService.Spreadsheets.Get(sheetId).ExecuteAsync();
        return spreadsheet.Sheets[0].Properties.Title;
    }

    /// <summary>
    /// Sütun indeksini harf gösterimine çevirir. Örn: 0 → "A", 8 → "I"
    /// </summary>
    private static string ColumnIndexToLetter(int index)
    {
        return ((char)('A' + index)).ToString();
    }

    /// <summary>
    /// Katılımcı bilgilerini belirtilen Sheets'e satır olarak ekler.
    /// </summary>
    public async Task AddParticipantAsync(string sheetId, SheetParticipantDTO participantData)
    {
        var sheetTitle = await GetFirstSheetTitleAsync(sheetId);

        // Tablodaki sütun sırasına göre: A B C D E F G H
        var newRow = new List<object>
        {
            participantData.JoinDate.ToString("yyyy-MM-dd HH:mm:ss K"), // A - Katılım Tarihi
            participantData.Name,                                         // B - Adı
            participantData.Surname,                                      // C - Soyadı
            participantData.UniversityName,                               // D - Üniversite
            participantData.DepartmentName,                               // E - Bölüm
            participantData.Graduaiton_Date.ToString() ?? "",                      // F - Kaçıncı Sınıf
            participantData.Email,                                        // G - Email
            participantData.IsJoinedForSponsor ? "Evet" : "Hayır"        // H - Sponsor Katılımı
            // I (CheckIn) sütunu başlangıçta boş bırakılır
        };

        var valueRange = new ValueRange { Values = new List<IList<object>> { newRow } };

        var appendRequest = _sheetsService.Spreadsheets.Values.Append(
            valueRange, sheetId, $"{sheetTitle}!A:H");

        appendRequest.ValueInputOption =
            SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

        await appendRequest.ExecuteAsync();
    }

    /// <summary>
    /// Belirtilen e-posta adresine sahip katılımcıyı Sheets'ten siler.
    /// </summary>
    public async Task RemoveParticipantAsync(string sheetId, string email)
    {
        var sheetTitle = await GetFirstSheetTitleAsync(sheetId);
        var range = $"{sheetTitle}!A:H";

        // 1. Tüm veriyi oku
        var getRequest = _sheetsService.Spreadsheets.Values.Get(sheetId, range);
        var response = await getRequest.ExecuteAsync();
        var values = response.Values;

        if (values == null || values.Count == 0)
            return;

        // 2. Silinecek satırı bul (başlık satırı 0. indeks, veri 1'den başlar)
        int rowIndexToDelete = -1;

        for (int i = 1; i < values.Count; i++)
        {
            var row = values[i];

            if (row.Count > ColEmail &&
                row[ColEmail]?.ToString().Equals(email, StringComparison.OrdinalIgnoreCase) == true)
            {
                rowIndexToDelete = i + 1; // Sheets API 1-tabanlı satır numarası
                break;
            }
        }

        if (rowIndexToDelete == -1)
            return; // Kayıt bulunamadı, sessizce çık

        // 3. Satırı sil (BatchUpdate — 0-tabanlı indeks ister)
        var deleteRequest = new BatchUpdateSpreadsheetRequest
        {
            Requests = new List<Request>
            {
                new Request
                {
                    DeleteDimension = new DeleteDimensionRequest
                    {
                        Range = new DimensionRange
                        {
                            SheetId = 0,
                            Dimension = "ROWS",
                            StartIndex = rowIndexToDelete - 1, // 0-tabanlı
                            EndIndex = rowIndexToDelete        // exclusive
                        }
                    }
                }
            }
        };

        await _sheetsService.Spreadsheets.BatchUpdate(deleteRequest, sheetId).ExecuteAsync();
    }

    /// <summary>
    /// Belirtilen kullanıcıyı Sheets'te "Checked In" olarak işaretler
    /// ve I sütunundaki hücreyi yeşil renkle boyar.
    /// </summary>
    public async Task MarkUserAsCheckedInAsync(string sheetId, string userEmail)
    {
        var sheetTitle = await GetFirstSheetTitleAsync(sheetId);
        var range = $"{sheetTitle}!A:{ColumnIndexToLetter(ColCheckIn)}"; // A:I

        // 1. Tüm veriyi oku
        var request = _sheetsService.Spreadsheets.Values.Get(sheetId, range);
        var response = await request.ExecuteAsync();
        var values = response.Values;

        if (values == null || !values.Any())
            return;

        // 2. Hedef satırı bul
        int targetRowIndex = -1;

        for (int i = 1; i < values.Count; i++)
        {
            if (values[i].Count > ColEmail &&
                values[i][ColEmail] is string emailValue &&
                emailValue.Equals(userEmail, StringComparison.OrdinalIgnoreCase))
            {
                targetRowIndex = i; // 0-tabanlı (Sheets'te i+1. satır)
                break;
            }
        }

        if (targetRowIndex == -1)
            return; // Kullanıcı bulunamadı

        // 3. I sütununa "Checked In" yaz
        string cellAddress = $"{sheetTitle}!{ColumnIndexToLetter(ColCheckIn)}{targetRowIndex + 1}";

        var textValueRange = new ValueRange
        {
            Values = new List<IList<object>> { new List<object> { "Checked In" } }
        };

        var updateValueRequest = _sheetsService.Spreadsheets.Values.Update(
            textValueRange, sheetId, cellAddress);

        updateValueRequest.ValueInputOption =
            SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

        await updateValueRequest.ExecuteAsync();

        // 4. Hücreyi yeşil renkle boya
        var batchUpdateRequest = new BatchUpdateSpreadsheetRequest
        {
            Requests = new List<Request>
            {
                new Request
                {
                    UpdateCells = new UpdateCellsRequest
                    {
                        Fields = "userEnteredFormat.backgroundColor",
                        Rows = new List<RowData>
                        {
                            new RowData
                            {
                                Values = new List<CellData>
                                {
                                    new CellData
                                    {
                                        UserEnteredFormat = new CellFormat
                                        {
                                            BackgroundColor = new Color
                                            {
                                                Red   = 0.5f,
                                                Green = 1.0f,
                                                Blue  = 0.5f
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        Range = new GridRange
                        {
                            SheetId          = 0,
                            StartRowIndex    = targetRowIndex,
                            EndRowIndex      = targetRowIndex + 1,
                            StartColumnIndex = ColCheckIn,
                            EndColumnIndex   = ColCheckIn + 1
                        }
                    }
                }
            }
        };

        await _sheetsService.Spreadsheets.BatchUpdate(batchUpdateRequest, sheetId).ExecuteAsync();
    }
}