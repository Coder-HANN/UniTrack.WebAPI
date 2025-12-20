using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using UniTrack.Application.Abstraction.Services.Sheets;

namespace UniTrack.Infrastructure.Services.Sheets;

public class ParticipantSheetRepository : IParticipantSheetRepository
{
    private readonly SheetsService _sheetsService;

    // Verinin yazılacağı/okunacağı aralık (Sheets'in ilk sayfası)
    private const string SheetRange = "Sayfa1!A:Z";

    public ParticipantSheetRepository(GoogleCredential credential)
    {
        // SheetsService'in DI ile hazır geldiğini varsayıyoruz.
        _sheetsService = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "UniTrack CampusConnect App",
        });
    }

    /// <summary>
    /// Katılımcı bilgilerini belirtilen Sheets'e satır olarak ekler.
    /// </summary>
    public async Task AddParticipantAsync(string sheetId, SheetParticipantDTO participantData)
    {
        var newRow = new List<object>
        {
            participantData.JoinDate.ToString("yyyy-MM-dd HH:mm:ss K"),
            participantData.Name,
            participantData.Surname,
            participantData.UniversityName,
            participantData.DepartmentName,
            participantData.Email,
            participantData.PhoneNumber,
            participantData.IsJoinedForSponsor ? "Evet" : "Hayır"
        };

        var valueRange = new ValueRange { Values = new List<IList<object>> { newRow } };

        // 2. Sheets API çağrısı: Veriyi en sona ekle (APPEND)
        var appendRequest = _sheetsService.Spreadsheets.Values.Append(valueRange, sheetId, SheetRange);
        // Kullanıcının girdiği veri formatında işlenmesini sağlar (tarih, sayı formatları için önemlidir)
        appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

        await appendRequest.ExecuteAsync();
    }

    /// <summary>
    /// Belirtilen e-posta adresine sahip katılımcıyı Sheets'ten siler.
    /// </summary>
    public async Task RemoveParticipantAsync(string sheetId, string email)
    {
        // 1. Sheets'teki tüm veriyi oku (Silme işlemi için satır numarasını bulmalıyız)
        var getRequest = _sheetsService.Spreadsheets.Values.Get(sheetId, SheetRange);
        var response = await getRequest.ExecuteAsync();
        var values = response.Values;

        if (values == null || values.Count == 0)
        {
            // Kayıt bulunamadı. Silinecek bir şey yok.
            return;
        }

        // 2. Silinecek satırın indeksini (sıra numarasını) bulma
        const int emailColumnIndex = 6;

        int rowIndexToDelete = -1;

        for (int i = 1; i < values.Count; i++) // Başlık satırını (0) atlıyoruz
        {
            var row = values[i];

            // Satırın en azından E-posta sütununa kadar dolu olduğundan emin ol
            if (row.Count > emailColumnIndex)
            {
                // İndeks 6'daki değeri (Email) aradığımız e-posta ile karşılaştır
                if (row[emailColumnIndex]?.ToString().Equals(email, StringComparison.OrdinalIgnoreCase) == true)
                {
                    rowIndexToDelete = i + 1; // Sheets API satır numarası (1-tabanlı)
                    break;
                }
            }

            if (rowIndexToDelete != -1)
            {
                // 3. Silme İsteği (Batch Update Request ile satır silme)
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
                                    SheetId = 0, // Varsayılan Sheet ID (genellikle ilk sayfa 0'dır)
                                    Dimension = "ROWS", // Satır siliyoruz
                                    StartIndex = rowIndexToDelete - 1, // API 0-tabanlı başlangıç indeksi ister
                                    EndIndex = rowIndexToDelete // API 0-tabanlı bitiş indeksi ister
                                }
                            }
                        }
                    }
                };

                await _sheetsService.Spreadsheets.BatchUpdate(deleteRequest, sheetId).ExecuteAsync();
            }
            // Eğer kayıt bulunamazsa, işlem sessizce tamamlanır.
        }
    }

    /// <summary>
    /// Sheets API, 0 tabanlı indeksi (örneğin A=0, B=1) harfe çevirir (örneğin I=8).
    /// </summary>
    private string ColumnIndexToLetter(int index)
    {
        // ASCII 'A' = 65. İndeksi ekleyerek harfi buluruz.
        return ((char)('A' + index)).ToString();
    }

    /// <summary>
    /// Belirtilen kullanıcıyı Sheets'te "Checked In" olarak işaretler ve hücreyi yeşil renkle boyar.
    /// </summary>
    /// <param name="sheetId"></param>
    /// <param name="userEmail"></param>
    /// <returns></returns>
    public async Task MarkUserAsCheckedInAsync(string sheetId, string userEmail)
    {
        // E-posta'nın Sheets'te hangi sütunda olduğunu varsayalım (G sütunu = 6. indeks)
        const int EmailColumnIndex = 6;

        // Boyanacak sütunun indeksi (I sütunu = 8. indeks, bu sütunda Check-In durumu tutuluyor)
        const int CheckInColumnIndex = 8;

        // --- 1. Sheets'ten mevcut tüm verileri alarak hedef satırı bulma ---

        // A:I aralığını oku
        var range = "Sayfa1!A:I";
        var request = _sheetsService.Spreadsheets.Values.Get(sheetId, range);
        var response = await request.ExecuteAsync();
        var values = response.Values;

        // Veri yoksa veya boşsa işlemi sonlandır
        if (values == null || !values.Any())
            return;

        int targetRowIndex = -1;
        for (int i = 1; i < values.Count; i++) // Başlık satırını (0. indeks) atla
        {
            // E-posta sütununda geçerli bir değer olup olmadığını kontrol et
            if (values[i].Count > EmailColumnIndex && values[i][EmailColumnIndex] is string emailValue)
            {
                // Kullanıcının E-posta adresiyle eşleşen satırı bul
                if (emailValue.Equals(userEmail, StringComparison.OrdinalIgnoreCase))
                {
                    targetRowIndex = i; // 0 tabanlı satır indeksi (Sheets'te bu i + 1. satırdır)
                    break;
                }
            }
        }

        if (targetRowIndex == -1)
        {
            // Kullanıcı Sheets tablosunda bulunamadı
            return;
        }

        // --- 2. Hücreye "Checked In" Metnini Yazma (Values.Update) ---

        // Güncellenecek hücrenin adını oluştur (Örn: I + (targetRowIndex + 1))
        string cellToUpdate = $"Sayfa1!{ColumnIndexToLetter(CheckInColumnIndex)}{targetRowIndex + 1}";

        var textValueRange = new ValueRange();
        // Hücreye yazılacak metin
        textValueRange.Values = new List<IList<object>> { new List<object> { "Checked In" } };

        var updateValueRequest = _sheetsService.Spreadsheets.Values.Update(textValueRange, sheetId, cellToUpdate);
        updateValueRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

        // Metin güncelleme isteğini çalıştır
        await updateValueRequest.ExecuteAsync();


        // --- 3. Hücreyi Yeşil Renkle Boyama (BatchUpdate) ---

        var batchUpdateRequest = new BatchUpdateSpreadsheetRequest();

        batchUpdateRequest.Requests = new List<Request>
{
    new Request
    {
        UpdateCells = new UpdateCellsRequest
        {
            // Hücre formatının sadece arkaplan rengini güncellemek istediğimizi belirtiyoruz
            Fields = "userEnteredFormat.background",
            
            // Formatın uygulanacağı hücre şablonu
            Rows = new List<RowData>
            {
                new RowData
                {
                    Values = new List<CellData>
                    {
                        new CellData
                        {
                            // Renk Tanımı
                            UserEnteredFormat = new CellFormat
                            {
                                BackgroundColor = new Color
                                {
                                    Red = 0.5f,
                                    Green = 1.0f, // Tam yeşil
                                    Blue = 0.5f
                                }
                            }
                        }
                    }
                }
            },
            
            // Uygulanacak Hücre Aralığı
            Range = new GridRange
            {
                SheetId = 0,
                StartRowIndex = targetRowIndex,
                EndRowIndex = targetRowIndex + 1,
                StartColumnIndex = CheckInColumnIndex,
                EndColumnIndex = CheckInColumnIndex + 1
            }
        }
    }
    };

        // Renklendirme isteğini çalıştır
        var batchRequest = _sheetsService.Spreadsheets.BatchUpdate(batchUpdateRequest, sheetId);
        await batchRequest.ExecuteAsync();
    }
}