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

        internal LogChannel(Stream stream)
            : this(stream, Encoding.Default, 30_000) {}

        internal LogChannel(Stream stream, Encoding encoding)
            : this(stream, encoding, 30_000) {}

        internal LogChannel(Stream stream, Encoding encoding, int capacity)
            : this(new StreamWriter(stream, encoding) { AutoFlush = false }, capacity) {}

        internal LogChannel(TextWriter writer) : this(writer, 30_000) {}

        internal LogChannel(TextWriter writer, int capacity)
        {
            this.queue = new BlockingCollection<string>(capacity);

            this.writer = writer;

            this.task = Task.Run(() =>
            {
                foreach(var log in this.queue.GetConsumingEnumerable())
                {
                    this.writer.WriteLine(log);
                }

                return this.writer.FlushAsync();
            });
        }

        public void Send(string log)
        {
            this.queue.Add(log);
        }

        public bool TrySend(string log, int waitMillisecond = 200)
        {
            return this.queue.TryAdd(log, waitMillisecond);
        }

        public void Close()
        {
            this.queue.CompleteAdding();

            this.task.ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            this.writer.Close();
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref this.disposed, 1) != 0) return;

            this.queue.CompleteAdding();

            this.task.ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            this.writer.Dispose();
        }
    }
}