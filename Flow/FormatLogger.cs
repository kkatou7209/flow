using System;
using System.Threading.Tasks;

namespace Flow
{
    public delegate string LogFormatter<T>(T log);

    public sealed class FormatLogger<T> : IDisposable
    {
        private readonly Logger logger;

        private readonly LogFormatter<T> formatter;

        public FormatLogger(Logger logger, LogFormatter<T> formatter)
        {
            if (logger is null)
                throw new ArgumentNullException(nameof(logger));

            if (formatter is null)
                throw new ArgumentNullException(nameof(formatter));

            this.logger = logger;
            this.formatter = formatter;
        }

        public void Dispose()
        {
            this.logger.Dispose();
        }

        public void Log(T log)
        {
            this.logger.Log(this.formatter(log));
        }

        public Task LogAsync(T log)
        {
            return this.logger.LogAsync(this.formatter(log));
        }
    }
}