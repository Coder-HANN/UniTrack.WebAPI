using Microsoft.Extensions.Localization;
using System.Globalization;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Localization.Resources;

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

        public async Task<string> Get(string key)
        {
            var localizer = factory.Create(typeof(Resources.ValidationMessages));
            return localizer[key];
        }

    }
}
