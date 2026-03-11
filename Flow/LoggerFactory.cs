using Flow.FileLoggers.Builders;

namespace Flow;

#pragma warning disable CA1822 // Mark members as static
/// <summary>
/// Factory of loggers.
/// </summary>
public class LoggerFactory
{
    public FileLoggerBuiler ForFile(string path) => new(path);
}
#pragma warning restore CA1822 // Mark members as static