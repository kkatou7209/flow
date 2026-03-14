using System.Text;
using NFluent;

namespace Flow.Tests.Unit;

public sealed class FormatFanOutLoggerTest : IDisposable
{
    private readonly string[] paths = [.. Enumerable.Range(1, 200).Select(_ => Path.GetTempFileName())];

    [Fact]
    public async Task Should_Log_Multiple_File_Formatted()
    {
        var loggers = new List<FormatLogger<dynamic>>(paths.Length);

        static string formatter(dynamic obj)
        {
            return $"[{obj.DateTime?.ToString("HH:mm:ss")}] {obj.Exception.GetType()?.Name}: {obj.Exception?.Message}";
        }

        foreach (var path in paths)
        {
            var _logger = new Logger(path, Encoding.Default, 4069, 300);
            loggers.Add(new FormatLogger<dynamic>(_logger, formatter));
        }

        var logger = new FormatFanOutLogger<dynamic>([.. loggers]);

        var datetime = DateTimeOffset.Parse("2020-01-09T12:43:18+09:00");

        var exception = new InvalidProgramException("Error occured.");

        await logger.LogAsync(new { DateTime = datetime, Exception = exception });

        logger.Dispose();

        foreach(var path in paths)
        {
            var log = File.ReadAllText(path);

            Check.That(log).IsEqualTo(
                """
                [12:43:18] InvalidProgramException: Error occured.

                """
            );
        }
    }

    public void Dispose()
    {
        foreach(var path in paths)
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}