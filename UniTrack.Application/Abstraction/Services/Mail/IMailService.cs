namespace UniTrack.Application.Abstraction.Services.Mail
{
    public interface IMailService
    {
        // Tuple yerine ValueTuple kullanmak için System.IO ve System.Threading.Tasks using'leri eklendi.
        Task SendMailAsync(string to, string subject, string body, bool isBodyHtml = true, List<(Stream Stream, string FileName)> attachments = null);

    }
}
