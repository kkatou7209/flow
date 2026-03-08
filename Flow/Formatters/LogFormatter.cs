namespace Flow.Formatters;

/// <summary>
/// Flow default log formatter.
/// </summary>
/// <param name="option">The options for formatting.</param>
internal sealed class LogFormatter(LogFormmatterOption option) : ILogFormatter
{
    public string Format(LogInfo info)
    {
        string formatted = option.Template;

        formatted = formatted.Replace(
            option.Placeholders.Message,
            info.Message
        );

        formatted = formatted.Replace(
            option.Placeholders.DateTime,
            option.DateTimeFormatter?.Invoke(info.DateTime) ?? string.Empty
        );

        formatted = formatted.Replace(
            option.Placeholders.Severity,
            option.SeverityLabels.TryGetValue(info.Severity, out var label) ? label : string.Empty
        );

        if (info.Exception is not null)
        {
            formatted = formatted.Replace(
                option.Placeholders.Exception,
                option.ExceptionFormatter?.Invoke(info.Exception) ?? string.Empty
            );
        }

        return formatted;
    }
}