namespace UniTrack.Domain.Enums
{
    public enum VerificationType
    {
        ClubRegistration = 0, // Kulüp Kayıt Doğrulaması
        UserRegistration = 1, // Kullanıcı Kayıt Doğrulaması
        PasswordReset = 2,    // Şifre Sıfırlama
        EmailChange = 3       // E-posta Değiştirme (İlerde lazım olabilir)
    }
}