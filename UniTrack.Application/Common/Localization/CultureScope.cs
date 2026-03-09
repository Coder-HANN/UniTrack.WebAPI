using System.Globalization;

namespace UniTrack.Application.Common.Localization
{
    public sealed class CultureScope : IDisposable
    {
        private readonly CultureInfo originalCulture;
        private readonly CultureInfo originalUICulture;

        public CultureScope(string culture)
        {
            originalCulture = CultureInfo.CurrentCulture;
            originalUICulture = CultureInfo.CurrentUICulture;

            // 1. Angular SSR'dan gelen "*" (yıldız) veya boş değerleri yakala
            if (string.IsNullOrWhiteSpace(culture) || culture == "*")
            {
                culture = "tr-TR"; // Varsayılan dili Türkçe yapıyoruz (veya en-US yapabilirsin)
            }

            try
            {
                var ci = new CultureInfo(culture);
                CultureInfo.CurrentCulture = ci;
                CultureInfo.CurrentUICulture = ci;
            }
            catch (CultureNotFoundException)
            {
                // 2. Eğer tanımlanamayan başka garip bir dil kodu gelirse uygulamanın çökmesini engelle
                var fallbackCi = new CultureInfo("tr-TR");
                CultureInfo.CurrentCulture = fallbackCi;
                CultureInfo.CurrentUICulture = fallbackCi;
            }
        }

        public void Dispose()
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUICulture;
        }
    }
}