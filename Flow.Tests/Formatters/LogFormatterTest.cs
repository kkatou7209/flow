using Flow.Formatters;

namespace Flow.Tests.Formatters;

public sealed class LogFormatterTest
{
    [Fact]
    public void Should_Format_By_Given_Option()
    {
        var option = new LogFormmatterOption()
        {
            Template = "{Time}|{Sev} {Msg} {Ex}",
            DateTimeFormatter = (datetime) => datetime.ToString("HH:mm:ss.fff"),
            ExceptionFormatter = (ex) => $"{ex.Message}",
            SeverityLabels = new(new Dictionary<Severity, string>()
            {
                [Severity.Debug]    = "SEV-DEBUG", 
                [Severity.Info]     = "SEV-INFO", 
                [Severity.Warning]  = "SEV-WARN", 
                [Severity.Error]    = "SEV-ERR", 
                [Severity.Critical] = "SEV-CRIT", 
                [Severity.Fatal]    = "SEV-FATAL", 
            }),
            Placeholders = new LogFormmatterOption.FormatPlaceholders()
            {
                Severity  = "{Sev}",
                DateTime  = "{Time}",
                Message   = "{Msg}",
                Exception = "{Ex}",
            },
        };

        var formatter = new LogFormatter(option);

        var info = new LogInfo()
        {
            DateTime  = DateTimeOffset.Parse("2026-03-07T23:15:42+09:00"),
            Exception = new Exception("Failed to connect to database."),
            Message   = "Error occured.",
            Severity  = Severity.Fatal,
        };

        var log = formatter.Format(info);

        Check.That(log)
            .IsEqualTo("23:15:42.000|SEV-FATAL Error occured. Failed to connect to database.");
    }

    [Fact]
    public void Should_Format_By_Default_Option()
    {
        var formatter = new LogFormatter(LogFormmatterOption.Default);

        var ex = new InvalidOperationException("Media type not supported.");

        var info = new LogInfo()
        {
            Message = "Requested Failed.",
            Severity = Severity.Warning,
            DateTime = DateTimeOffset.Parse("2026-03-10T12:07:35.888+09:00"),
            Exception = ex,
        };

        var log = formatter.Format(info);

        Check.That(log)
            .IsEqualTo(
                $"""
                [2026-03-10 12:07:35.888] [WARN] Requested Failed.:
                {ex.GetType().Name}: {ex.Message}
                {ex.StackTrace}
                """
            );
    }
}