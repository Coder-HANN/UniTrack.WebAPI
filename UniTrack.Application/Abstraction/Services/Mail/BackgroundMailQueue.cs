using System.Threading.Channels;

namespace UniTrack.Application.Abstraction.Services.Mail
{
    public class BackgroundMailQueue : IBackgroundMailQueue
    {
        private readonly Channel<MailQueueItem> _queue;

        public BackgroundMailQueue()
        {
            _queue = Channel.CreateUnbounded<MailQueueItem>(
                new UnboundedChannelOptions
                {
                    SingleReader = true,
                    SingleWriter = false
                });
        }

        public void Enqueue(MailQueueItem item)
        {
            _queue.Writer.TryWrite(item);
        }

        public ChannelReader<MailQueueItem> Reader => _queue.Reader;
    }

}
