namespace UniTrack.Application.Abstraction.Services.Sheets
{
    public interface IGoogleSheetCreationService
    {
        /// <summary>
        /// Yeni bir E-Tablo oluşturur ve başlık satırını ekler.
        /// </summary>
        /// <param name="sheetTitle">E-Tabloya verilecek başlık (Genellikle Etkinlik Adı).</param>
        /// <returns>Oluşturulan E-Tablo'nun benzersiz ID'si.</returns>
        Task<string> CreateSheetAsync(string eventId,string sheetTitle);
    }
}
