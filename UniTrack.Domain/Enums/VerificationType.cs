using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniTrack.Domain.Enums
{
    public enum VerificationType
    {
        ClubRegistration = 0, // Kulüp Kayıt Doğrulaması
        PasswordReset = 1,    // Şifre Sıfırlama
        EmailChange = 2       // E-posta Değiştirme (İlerde lazım olabilir)
    }
}