namespace UniTrack.Application.Abstraction.Services.Mail
{
    public class MailQueueItem
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; } = true;
    }

}
