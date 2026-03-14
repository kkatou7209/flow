using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Flow
{
    public sealed class Logger : ILogging, IDisposable
    {
        private readonly LogChannel channel;

        public Logger() : this(Console.Out) {}

        public Logger(TextWriter writer, int capacity = 50_000)
        {
            this.channel = new LogChannel(writer, capacity);
        }

        public Logger(string path, Encoding encoding = null, int bufferSize = 64_000, int capacity = 30_000)
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

        public void Log(string path, string log, Encoding encoding = null)
        {
            File.AppendAllLines(
                path,
                new string[] { log },
                encoding ?? Encoding.Default
            );
        }

        public async Task LogAsync(string log)
        {
            this.channel.Send(log);
        }

        public async Task LogAsync(string path, string log, Encoding encoding = null)
        {
            File.AppendAllLines(
                path,
                new string[] { log },
                encoding ?? Encoding.Default
            );
        }
    }
}