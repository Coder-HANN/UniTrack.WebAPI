using Microsoft.Extensions.Localization;
using System.Globalization;
using UniTrack.Application.Abstraction.Services.Localization;

namespace UniTrack.Application.Common.Localization
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IStringLocalizerFactory factory;
        private readonly ICurrentLanguageService currentLanguageService;

        public LocalizationService(
            IStringLocalizerFactory factory,
            ICurrentLanguageService currentLanguageService)
        {
            this.factory = factory;
            this.currentLanguageService = currentLanguageService;
        }

        public string Get(string key)
        {
            var culture = new CultureInfo(currentLanguageService.GetCulture());

            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            var localizer = factory.Create(typeof(ValidationMessages));
            return localizer[key];
        }
    }
}
