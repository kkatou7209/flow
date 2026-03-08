namespace Flow;

/// <summary>
/// The levels of logging.
/// </summary>
[Serializable]
public enum Severity
{
    /// <summary>
    /// Debug log level.
    /// </summary>
    Debug,
    /// <summary>
    /// Infomation log level.
    /// </summary>
    Info,
    /// <summary>
    /// Warning log level.
    /// </summary>
    Warning,
    /// <summary>
    /// Error log level.
    /// </summary>
    Error,
    /// <summary>
    /// Critical log level.
    /// </summary>
    Critical,
    /// <summary>
    /// Fatal log level.
    /// </summary>
    Fatal,
}