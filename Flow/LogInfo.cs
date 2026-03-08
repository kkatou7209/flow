namespace Flow;

/// <summary>
/// Infomation for logging.
/// </summary>
[Serializable]
public class LogInfo
{
    /// <summary>
    /// The log severity.
    /// </summary>
    public Severity Severity { get; init; } = Severity.Info;

    /// <summary>
    /// The log message.
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// The occured exception.
    /// </summary>
    public Exception? Exception { get; init; } = null;

    /// <summary>
    /// The log date and time.
    /// </summary>
    public DateTimeOffset DateTime { get; init; } = DateTimeOffset.Now;
}