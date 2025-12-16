namespace UniTrack.Application.Abstraction.Services.Localization
{
    public interface ILocalizationService
    {
        Task<string> Get(string key);
        Task<string> Get(string key, params object[] args);
    }
}
