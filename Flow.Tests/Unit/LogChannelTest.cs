namespace Flow.Tests.Unit;

using System.Text;
using Flow;
using NFluent;

public sealed class LogChannelTest : IDisposable
{
    private readonly string path = Path.GetTempFileName();

    private readonly string pathAsync = Path.GetTempFileName();

    [Fact(DisplayName = "LogChannel should write to file.", Timeout = 1000)]
    public async Task Should_Write_To_Log_File()
    {
        using var channel = new LogChannel(new FileStream(path, FileMode.Append), Encoding.Default, 2000);

        channel.Send("[12:30:37] GET /user/1 HTTP/2");
        channel.Send("[12:30:57] POST /user HTTP/2");
        channel.Send("[12:31:03] DELETE /user/2 HTTP/2");

        channel.Dispose();

        var log = File.ReadAllText(path);

        Check.That(log).IsEqualTo(
            """
            [12:30:37] GET /user/1 HTTP/2
            [12:30:57] POST /user HTTP/2
            [12:31:03] DELETE /user/2 HTTP/2

            """
        );
    }

    [Fact(DisplayName = "LogChannel should write to log file sequencialy", Timeout = 3000)]
    public async Task Should_Write_Logs_Sequencial_From_Multiple_Threads()
    {
        using var channel = new LogChannel(new FileStream(pathAsync, FileMode.Append), Encoding.Default, 2000);

        const string LOG = "Log from multiple thread";

        var tasks = new List<Task>();

        foreach(var i in Enumerable.Range(1, 10))
        {
            tasks.Add(Task.Run(() =>
            {
                foreach(var j in Enumerable.Range(1, 100))
                {
                    channel.Send(LOG);
                }
            }));
        }

        await Task.WhenAll(tasks);

        channel.Dispose();

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