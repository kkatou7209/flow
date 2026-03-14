using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Flow
{
    public sealed class Logger : IDisposable
    {
        private readonly LogChannel channel;

        public Logger(TextWriter writer, int capacity)
        {
            this.channel = new LogChannel(writer, capacity);
        }

        public Logger(Stream stream, Encoding encoding, int capacity)
            : this(
                new StreamWriter(
                    stream,
                    encoding
                ),
                capacity
            ) {}

        public Logger(string path, Encoding encoding, int bufferSize, int capacity)
            : this(
                new StreamWriter(
                    new FileStream(
                        path,
                        FileMode.Append,
                        FileAccess.Write,
                        FileShare.ReadWrite | FileShare.Delete,
                        bufferSize,
                        FileOptions.Asynchronous | FileOptions.SequentialScan
                    ),
                    encoding
                ),
                capacity
            ) {}
        
        public void Dispose()
        {
            this.channel.Dispose();
        }

        public void Log(string log)
        {
            this.channel.TrySend(log);
        }

        public async Task LogAsync(string log)
        {
            await Task.Run(() => this.channel.Send(log))
                .ConfigureAwait(false);
        }
    }
}