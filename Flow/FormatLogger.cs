using System;
using System.Threading.Tasks;

namespace Flow
{
    public sealed class FormatLogger<T> : IDisposable
    {
        private readonly Logger logger;
        private readonly Func<T, string> format;

        public FormatLogger(Logger logger, Func<T, string> format)
        {
            if (logger is null)
                throw new ArgumentNullException(nameof(logger));

            if (format is null)
                throw new ArgumentNullException(nameof(format));

            this.logger = logger;
            this.format = format;
        }

        public void Dispose()
        {
            this.logger.Dispose();
        }

        public void Log(T log)
        {
            this.logger.Log(this.format(log));
        }

        public async Task LogAsync(T log)
        {
            await this.logger.LogAsync(this.format(log));
        }
    }
}