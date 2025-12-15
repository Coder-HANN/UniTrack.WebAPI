using Serilog;

namespace UniTrack.Application.Abstraction.Services.Logger
{
    public abstract class LoggerServiceBase
    {
        protected ILogger Logger { get; set; }

        public void Info(string message) => Logger.Information(message);
        public void Warn(string message) => Logger.Warning(message);
        public void Error(string message) => Logger.Error(message);
        public void Error(string message, Exception ex) => Logger.Error(ex, message);
    }
}
