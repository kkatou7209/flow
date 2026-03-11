using System.Text;
using System.Threading.Channels;

namespace Flow.FileLoggers;

/// <summary>
/// Log worker for file logging.
/// </summary>
internal sealed class FileLogWorker : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Log message queue.
    /// </summary>
    private readonly Channel<string> queue;

    /// <summary>
    /// Logging task.
    /// </summary>
    private readonly Task task;

    /// <summary>
    /// Indicate that this object is disposed.
    /// </summary>
    private int disposed = 0;

    internal FileLogWorker(
        string path,
        int bufferSize,
        int capacity,
        long initialAllocationSize,
        Encoding encoding
    )
    {
        this.queue = Channel.CreateBounded<string>(new BoundedChannelOptions(capacity)
        {
            SingleReader = true,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.Wait
        });

        StreamWriter writer = File.Exists(path)
            ? new(
                new FileStream(path, new FileStreamOptions()
                {
                    Access            = FileAccess.Write,
                    BufferSize        = bufferSize,
                    Mode              = FileMode.Append,
                    Options           = FileOptions.Asynchronous | FileOptions.SequentialScan,
                    Share             = FileShare.ReadWrite | FileShare.Delete,
                }),
                encoding
            )
            : new(
                new FileStream(path, new FileStreamOptions()
                {
                    Access            = FileAccess.Write,
                    BufferSize        = bufferSize,
                    Mode              = FileMode.Append,
                    Options           = FileOptions.Asynchronous | FileOptions.SequentialScan,
                    Share             = FileShare.ReadWrite | FileShare.Delete,
                    PreallocationSize = initialAllocationSize,
                }),
                encoding
            );

        async Task process()
        {
            using var w = writer;

            await foreach (var log in this.queue.Reader.ReadAllAsync())
            {
                await w.WriteAsync(log);
            }

            await w.FlushAsync();
        }

        this.task = Task.Run(process);
    }

    /// <summary>
    /// Sends log to worker.
    /// </summary>
    public bool TrySend(string log)
    {
        if (Volatile.Read(ref disposed) != 0)
            return false;

        return this.queue.Writer.TryWrite(log);
    }

    /// <summary>
    /// Sends log to worker async.
    /// </summary>
    public ValueTask SendAsync(string log)
    {
        ObjectDisposedException.ThrowIf(
            Volatile.Read(ref this.disposed) != 0,
            this
        );

        return this.queue.Writer.WriteAsync(log);
    }

    public void Dispose()
    {
        this.DisposeAsync()
            .AsTask()
            .GetAwaiter()
            .GetResult();
    }

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref disposed, 1) != 0) return;

        this.queue.Writer.Complete();

        await this.task.ConfigureAwait(false);
    }
}