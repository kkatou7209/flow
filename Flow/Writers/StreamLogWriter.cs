using System.Text;

namespace Flow.Writers;

/// <summary>
/// Logger implementation of Stream
/// </summary>
/// <param name="stream">The stream to output log.</param>
internal sealed class StreamLogWriter(Stream stream, Encoding? encoding = null)
    : ILogWriter, IAsyncLogWriter, IDisposable
{
    private readonly StreamWriter writer = new(stream, encoding)
    {
        AutoFlush = true,
    };

    public void Write(string log)
    {
        writer.Write(log);
    }

    public Task WriteAsync(string log)
    {
        return writer.WriteAsync(log.ToString());
    }

    public void Dispose()
    {
        this.writer.Close();
    }
}