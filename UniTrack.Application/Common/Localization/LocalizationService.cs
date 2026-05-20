using Microsoft.Extensions.Localization;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common.Localization;
using UniTrack.Application.Common.Localization.Resources;

public class LocalizationService : ILocalizationService
{
    // factory yerine artık doğrudan strongly-typed localizer kullanıyoruz
    private readonly IStringLocalizer<ValidationMessages> _localizer;
    private readonly ICurrentLanguageService _currentLanguageService;

    public LocalizationService(
        IStringLocalizer<ValidationMessages> localizer,
        ICurrentLanguageService currentLanguageService)
    {
        _localizer = localizer;
        _currentLanguageService = currentLanguageService;
    }

    public async Task<string> Get(string key)
    {
        var culture = await _currentLanguageService.GetCultureAsync();

        using (new CultureScope(culture))
        {
            // .Value ekleyerek kesin string karşılığını alıyoruz
            return _localizer[key].Value;
        }
    }

    public async Task<string> Get(string key, params object[] args)
    {
        var culture = await _currentLanguageService.GetCultureAsync();

        using (new CultureScope(culture))
        {
            return _localizer[key, args].Value;
        }
    }
}