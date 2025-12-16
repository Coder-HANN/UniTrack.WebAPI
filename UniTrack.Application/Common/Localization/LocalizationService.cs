using Microsoft.Extensions.Localization;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Localization;
using UniTrack.Application.Common.Localization.Resources;

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
        var culture = await currentLanguageService.GetCultureAsync();

        using (new CultureScope(culture))
        {
            var localizer = factory.Create(typeof(ValidationMessages));
            return localizer[key];
        }
    }

    public async Task<string> Get(string key, params object[] args)
    {
        var culture = await currentLanguageService.GetCultureAsync();

        using (new CultureScope(culture))
        {
            var localizer = factory.Create(typeof(ValidationMessages));
            return localizer[key, args];
        }
    }
}
