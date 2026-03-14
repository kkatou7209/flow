using NFluent;

namespace Flow.Tests.Unit;

public sealed class FormatLoggerTest : IDisposable
{
    private readonly string path = Path.GetTempFileName();

    [Fact(Timeout = 2000)]
    public async Task Should_Logs_With_Formatting()
    {
        using var logger = new FormatLogger<dynamic>(
            new Logger(path),
            obj => $"[{obj.DateTime?.ToString("HH:mm:ss")}] {obj.Exception.GetType()?.Name}: {obj.Exception?.Message}"
        );

        var datetime = DateTimeOffset.Parse("2020-01-09T12:43:18+09:00");

        var exception = new InvalidProgramException("Error occured.");

        logger.Log(new { DateTime = datetime, Exception = exception });

        logger.Dispose();

        var log = File.ReadAllText(path);

        Check.That(log).IsEqualTo(
            """
            [12:43:18] InvalidProgramException: Error occured.
            
            """
        );
    }

    public void Dispose()
    {
        if (File.Exists(path))
            File.Delete(path);
    }
}