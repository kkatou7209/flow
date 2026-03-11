using System.Text;

namespace Flow.FileLoggers.Builders;

/// <summary>
/// Builder of FileLogger.
/// </summary>
public sealed class FileLoggerBuiler
{
    private readonly string path;

    private int bufferSize = FileLogger.Default.BUFFER_SIZE;

    private int capacity = FileLogger.Default.QUEUE_CAPACITY;

    private long allocationSize = FileLogger.Default.ALLOCATION_SIZE;

    private Encoding encoding = FileLogger.Default.ENCODING;

    internal FileLoggerBuiler(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        this.path = path;
    }

    /// <summary>
    /// Specifies stream buffer size.
    /// </summary>
    /// <param name="size">Buffer size.</param>
    public FileLoggerBuiler WithBufferSize(int size)
    {
        this.bufferSize = size;

        return this;
    }

    /// <summary>
    /// Specifies capacity of log queue.
    /// </summary>
    /// <param name="capacity">Capacity of log queue.</param>
    public FileLoggerBuiler WithCapacity(int capacity)
    {
        this.capacity = capacity;

        return this;
    }

    /// <summary>
    /// Specifies initial disk allocation size when creating file.
    /// </summary>
    /// <param name="size">Allocation size.</param>
    public FileLoggerBuiler WithAllocationSize(long size)
    {
        this.allocationSize = size;

        return this;
    }

    /// <summary>
    /// Specifies encoding of log text.
    /// </summary>
    /// <param name="encoding">Log file encoding.</param>
    public FileLoggerBuiler WithEncoding(Encoding encoding)
    {
        this.encoding = encoding;

        return this;
    }

    /// <summary>
    /// Instanciates <c>FileLogger</c> with given settings.
    /// </summary>
    public FileLogger Build()
    {
        return new(
            this.path,
            this.bufferSize,
            this.capacity,
            this.allocationSize,
            this.encoding
        );
    }
}