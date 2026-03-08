using System.Collections.ObjectModel;

namespace Flow.Formatters;

/// <summary>
/// Options for formatting.
/// </summary>
internal sealed class LogFormmatterOption
{
    /// <summary>
    /// The log template.
    /// </summary>
    public string Template { get; init; } = string.Empty;

    /// <summary>
    /// The labels for severity.
    /// </summary>
    public ReadOnlyDictionary<Severity, string> SeverityLabels { get; init; } = new (new Dictionary<Severity, string>());

    /// <summary>
    /// The function to format DateTimeOffset.
    /// </summary>
    public Func<DateTimeOffset, string>? DateTimeFormatter { get; init; } = null;

    /// <summary>
    /// The function to format Exception.
    /// </summary>
    public Func<Exception, string>? ExceptionFormatter { get; init; } = null;

    /// <summary>
    /// The template placeholders.
    /// </summary>
    public FormatPlaceholders Placeholders { get; init; } = new();

    /// <summary>
    /// Placeholders of template.
    /// </summary>
    internal class FormatPlaceholders
    {
        /// <summary>
        /// DateTimeOffset placeholder. Default: "{DateTime}"
        /// </summary>
        public string DateTime { get; init; } = string.Empty;

        /// <summary>
        /// Exception placeholder. Default: "{Exception}"
        /// </summary>
        public string Exception { get; init; } = string.Empty;

        /// <summary>
        /// Message placeholder. Default: "{Message}"
        /// </summary>
        public string Message { get; init; } = string.Empty;

        /// <summary>
        /// Severity placeholder. Default: "{Severity}"
        /// </summary>
        public string Severity { get; init; } = string.Empty;

        /// <summary>
        /// The default placeholders.
        /// </summary>
        public static readonly FormatPlaceholders Default = new()
        {
            DateTime  = "{DateTime}",
            Exception = "{Exception}",
            Message   = "{Message}",
            Severity  = "{Severity}",
        };
    }

    /// <summary>
    /// LogFormmater default option.
    /// </summary>
    internal static readonly LogFormmatterOption Default = new()
    {
        Template = "[{DateTime}] [{Severity}] {Message}{Exception}",
        SeverityLabels = new (new Dictionary<Severity, string>()
        {
            [Severity.Debug] = "DEBUG", 
            [Severity.Info] = "INFO", 
            [Severity.Warning] = "WARN", 
            [Severity.Error] = "ERROR", 
            [Severity.Critical] = "CRITICAL", 
            [Severity.Fatal] = "FATAL", 
        }),
        DateTimeFormatter = (datetime) => datetime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
        ExceptionFormatter = (ex) => $":{Environment.NewLine}{ex.GetType().Name}: {ex.Message}{Environment.NewLine}{ex.StackTrace}",
        Placeholders = FormatPlaceholders.Default,
    };
}
