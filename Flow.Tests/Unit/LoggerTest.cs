using System.Text;
using NFluent;

namespace Flow.Tests.Unit;

public sealed class LoggerTest : IDisposable
{
    private readonly string path = Path.GetTempFileName();

    private readonly string pathAsync = Path.GetTempFileName();

    [Fact]
    public void Should_Log_To_File()
    {
        using var logger = new Logger(path, Encoding.Default, 4069, 10);

        logger.Log("[12:30:37] GET /user/1 HTTP/2");
        logger.Log("[12:30:54] DELETE /user/4 HTTP/2");

        logger.Dispose();

        var log = File.ReadAllText(path);

        Check.That(log).IsEqualTo(
            """
            [12:30:37] GET /user/1 HTTP/2
            [12:30:54] DELETE /user/4 HTTP/2

            """
        );
    }

    [Fact(Timeout = 3000)]
    public async Task Should_Log_Sequencially_From_Multiple_Thread()
    {
        using var logger = new Logger(pathAsync, Encoding.Default, 4069, 1200);

        const string LOG = "Log from multiple thread";

        var tasks = new List<Task>();

        foreach(var i in Enumerable.Range(1, 10))
        {
            tasks.Add(Task.Run(async () =>
            {
                foreach(var j in Enumerable.Range(1, 100))
                {
                    await logger.LogAsync(LOG);
                }
            }));
        }

        await Task.WhenAll(tasks);

        logger.Dispose();

        var builder = new StringBuilder(1000 * LOG.Length + 1000);

        foreach(var line in Enumerable.Repeat(LOG, 1000))
        {
            builder.AppendLine(line);
        }

        var log = File.ReadAllText(pathAsync);        

        Check.That(log).IsEqualTo(builder.ToString());
    }

    public void Dispose()
    {
        if (File.Exists(path))
            File.Delete(path);
            
        if (File.Exists(pathAsync))
            File.Delete(pathAsync);

    }
}