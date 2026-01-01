using System.Threading.Channels;

namespace UniTrack.Application.Abstraction.Services.Mail
{
    public interface IBackgroundMailQueue
    {
        void Enqueue(MailQueueItem item);
        ChannelReader<MailQueueItem> Reader { get; }
    }

}
