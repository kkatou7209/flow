namespace Flow.Writers;

public interface IAsyncLogWriter
{
    /// <summary>
    /// Writes logs asynchronously.
    /// </summary>
    /// <param name="log">The log message.</param>
    Task WriteAsync(string log);
}