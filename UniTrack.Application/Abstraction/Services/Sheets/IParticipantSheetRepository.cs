namespace UniTrack.Application.Abstraction.Services.Sheets
{
    public interface IParticipantSheetRepository
    {
        /// <summary>
        /// Belirtilen E-Tabloya yeni bir katılımcı satırı ekler. (Yazma)
        /// </summary>
        /// <param name="sheetId">Etkinliğe ait E-Tablo'nun ID'si.</param>
        /// <param name="participantData">Sheets'e yazılacak katılımcı verisi.</param>
        Task AddParticipantAsync(string sheetId, SheetParticipantDTO participantData);

        /// <summary>
        /// Belirtilen E-Tablodan, verilen kullanıcı ID'sine sahip satırı bulur ve siler. (Silme)
        /// </summary>
        /// <param name="sheetId">Etkinliğe ait E-Tablo'nun ID'si.</param>
        /// <param name="Email">Silinecek kullanıcının benzersiz kimliği.</param>
        Task RemoveParticipantAsync(string sheetId, string Email);

        Task MarkUserAsCheckedInAsync(string sheetId, string userEmail);
    }
}
