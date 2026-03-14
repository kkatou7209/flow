using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Flow
{
    /// <summary>
    /// Logger that provide logs to all inner loggers.
    /// </summary>
    public sealed class FanOutLogger : IDisposable
    {
        private readonly ReadOnlyCollection<Logger> loggers;

        public FanOutLogger(params Logger[] loggers)
        {
            this.loggers = new ReadOnlyCollection<Logger>(loggers);
        }

        public void Dispose()
        {
            foreach (var log in loggers)
            {
                log.Dispose();
            }
        }

        public void Log(string log)
        {
            foreach(var logger in loggers)
            {
                logger.Log(log);
            }
        }

        public async Task LogAsync(string log)
        {
            var tasks = new List<Task>();

            foreach(var logger in loggers)
            {
                tasks.Add(logger.LogAsync(log));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}