namespace UniTrack.Application.Abstraction.Services.Localization
{
    public interface ICurrentLanguageService
    {
        Task<string> GetCultureAsync();
    }
}
