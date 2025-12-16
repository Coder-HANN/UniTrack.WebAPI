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

            var ci = new CultureInfo(culture);
            CultureInfo.CurrentCulture = ci;
            CultureInfo.CurrentUICulture = ci;
        }

        public void Dispose()
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUICulture;
        }
    }

}
