using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Flow
{
    internal sealed class LogChannel : IDisposable
    {
        private readonly TextWriter writer;

        private readonly Task task;

        private readonly BlockingCollection<string> queue;

        private int disposed = 0;

        internal LogChannel(Stream stream, Encoding encoding, int capacity)
            : this(new StreamWriter(stream, encoding) { AutoFlush = false }, capacity) {}

        internal LogChannel(TextWriter writer, int capacity)
        {
            this.queue = new BlockingCollection<string>(capacity);

            this.writer = writer;

            this.task = Task.Run(async () =>
            {
                foreach(var log in this.queue.GetConsumingEnumerable())
                {
                    this.writer.WriteLine(log);
                }
            });
        }

        public void Send(string log)
        {
            if (Volatile.Read(ref disposed) != 0)
                throw new ObjectDisposedException($"{nameof(LogChannel)} is disposed.");

            this.queue.Add(log);
        }

        public bool TrySend(string log, int waitMillisecond = 200)
        {
            if (Volatile.Read(ref disposed) != 0)
                throw new ObjectDisposedException($"{nameof(LogChannel)} is disposed.");

            return this.queue.TryAdd(log, waitMillisecond);
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref this.disposed, 1) != 0) return;

            this.queue.CompleteAdding();

            this.task.ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            this.writer.Flush();
            this.writer.Dispose();
        }
    }
}