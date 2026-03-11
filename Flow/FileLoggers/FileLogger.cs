using System.Text;

namespace Flow.FileLoggers;

/// <summary>
/// Logger that logs to file.
/// </summary>
public sealed class FileLogger : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// File logging worker.
    /// </summary>
    private readonly FileLogWorker worker;

    /// <summary>
    /// Indicates that this object is disposed.
    /// </summary>
    private int disposed = 0;

    /// <param name="path">Path to log file.</param>
    /// <param name="bufferSize">Buffer size of stream</param>
    /// <param name="capacity">Capacity of log queue.</param>
    /// <param name="initialAllocationSize">Initial disk allocation size.</param>
    /// <param name="encoding">Encoding of file.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public FileLogger(
        string path,
        int bufferSize             = Default.BUFFER_SIZE,
        int capacity               = Default.QUEUE_CAPACITY,
        long initialAllocationSize = Default.ALLOCATION_SIZE,
        Encoding? encoding         = null
    )
    {
        ArgumentNullException.ThrowIfNull(path);

        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Fiel path cannot be blank.", nameof(path));

        if (int.IsNegative(bufferSize))
            throw new ArgumentOutOfRangeException(nameof(bufferSize), bufferSize, "Buffer size cannot be negative.");

        if (int.IsNegative(capacity))
            throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "Queue capacity cannot be negative.");

        if (long.IsNegative(initialAllocationSize))
            throw new ArgumentOutOfRangeException(nameof(initialAllocationSize), initialAllocationSize, "Allocation size cannot be negative.");

        worker = new FileLogWorker(
            Path.GetFullPath(path),
            bufferSize,
            capacity,
            initialAllocationSize,
            encoding ?? Default.ENCODING
        );
    }

    /// <summary>
    /// Logs to file.
    /// <para>
    /// Log may failed and dropped if queue is full.
    /// If you want to ensure to complete writing, use
    /// <c>LogAsync</c> instead.
    /// </para>
    /// </summary>
    /// <param name="log">Log message</param>
    public void Log(string log)
    {
        ObjectDisposedException.ThrowIf(this.disposed != 0, this);

        this.worker.TrySend(log);
    }

    /// <summary>
    /// Logs to file asynchroonously
    /// </summary>
    /// <param name="log">Log messsage.</param>
    /// <returns></returns>
    public ValueTask LogAsync(string log)
    {
        ObjectDisposedException.ThrowIf(this.disposed != 0, this);

        return this.worker.SendAsync(log);
    }

    public void Dispose()
    {
        this.DisposeAsync()
            .AsTask()
            .GetAwaiter()
            .GetResult();
    }

    public ValueTask DisposeAsync()
    {
        ObjectDisposedException.ThrowIf(
            Interlocked.Exchange(ref disposed, 1) != 0,
            this
        );

        return this.worker.DisposeAsync();
    }

    /// <summary>
    /// FileLogger constants.
    /// </summary>
    internal static class Default
    {
        /// <summary>
        /// Default stream buffer size.
        /// </summary>
        public const int BUFFER_SIZE = 64_000;
        
        /// <summary>
        /// Default queue capacity.
        /// </summary>
        public const int QUEUE_CAPACITY = 30_000;

        /// <summary>
        /// Default chaarchtor encoding.
        /// </summary>
        public readonly static Encoding ENCODING = Encoding.UTF8;

        /// <summary>
        /// Default allocation size of disk.
        /// </summary>
        public const long ALLOCATION_SIZE = 100_000;
    }
}