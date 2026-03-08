namespace Flow.Writers;

public interface ILogWriter
{
    /// <summary>
    /// Writes logs.
    /// </summary>
    /// <param name="log">The log message.</param>
    void Write(string log);
}