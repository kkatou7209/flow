using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Flow
{
    public sealed class FormatFanOutLogger<T> : IDisposable
    {
        private readonly ReadOnlyCollection<FormatLogger<T>> loggers;

        public FormatFanOutLogger(params FormatLogger<T>[] loggers)
        {
            this.loggers = new ReadOnlyCollection<FormatLogger<T>>(loggers);
        }

        public void Dispose()
        {
            foreach(var logger in loggers)
            {
                logger.Dispose();
            }
        }

        public void Log(T log)
        {
            foreach(var logger in loggers)
            {
                logger.Log(log);
            }
        }

        public Task LogAsync(T log)
        {
            var tasks = new List<Task>();

            foreach(var logger in loggers)
            {
                tasks.Add(logger.LogAsync(log));
            }

            return Task.WhenAll(tasks);
        }
    }
}