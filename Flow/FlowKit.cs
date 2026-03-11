using Flow.Loggers.Files.Builders;

namespace Flow;

/// <summary>
/// The Flow library API.
/// </summary>
public static class FlowKit
{
    /// <summary>
    /// Creates logger for logging to file.
    /// </summary>
    /// <param name="path">Path to log file.</param>
    /// <returns></returns>
    public static LoggerFactory Create() => new();
}