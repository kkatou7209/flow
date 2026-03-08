namespace Flow.Formatters;

/// <summary>
/// Formatter for log.
/// </summary>
public interface ILogFormatter
{
    /// <summary>
    /// Format log by given data and returns formatted string.
    /// </summary>
    public string Format(LogInfo info);
}