namespace UniTrack.Domain.Enums
{
    public enum NotificationType
    {
        EventCreated = 1,      // Kulüp yeni etkinlik eklediğinde
        EventUpdated = 2,      // Katılımcılara etkinlik güncellendiğinde
        EventDeleted = 3,      // Katılımcılara etkinlik silindiğinde
        EventReminder = 4,     // 24 saat önce gönderilen hatırlatıcı
        // Kullanıcı/Sistem Bazlı
        SystemMessage = 5,     // Genel sistem duyuruları (örn: SendToAllAsync)
        ClubFollowerGoal = 6,  // (Gelecek planı) Kulüp bir takipçi hedefine ulaştığında
        AddsMessage = 7,   // Reklam mesajları
        MentionsMessage = 8   // Bahsetme mesajları
    }
}