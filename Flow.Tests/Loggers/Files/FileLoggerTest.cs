using System.Text;
using Flow.Loggers.Files;

namespace Flow.Tests.Loggers.Files;

public sealed class FileLoggerTest : IDisposable
{
    private readonly string path = Path.GetTempFileName();

    private readonly string path2 = Path.GetTempFileName();


    [Fact]
    public async Task Should_Log_Work()
    {
        using (var logger = new FileLogger(path))
        {
            logger.Log("Requested by: ");

            await logger.LogAsync("User 1.");
        }

        var log = File.ReadAllText(path);

        Check.That(log).IsEqualTo("Requested by: User 1.");
    }

    [Fact]
    public async Task Should_Log_From_Multiple_Threads()
    {
        const string MESSAGE = "Hello";

        const int TASK_COUNT = 20;

        const int WRITE_COUNT = 100;

        var encoding = new UTF8Encoding(false);

        var byteLength = encoding.GetByteCount(MESSAGE);

        var writtenByteLength = byteLength * TASK_COUNT * WRITE_COUNT;

        using (var logger = new FileLogger(
            path2,
            bufferSize: 1_000,
            capacity: 50_000,
            initialAllocationSize: 10_000,
            encoding: Encoding.Default
        ))
        {
            var tasks = new List<Task>();

            foreach(var _ in Enumerable.Range(1, TASK_COUNT))
            {
                foreach(var __ in Enumerable.Range(1, WRITE_COUNT))
                {
                    tasks.Add(Task.Run(async () => await logger.LogAsync(MESSAGE)));
                }
            }

            await Task.WhenAll(tasks);
        };

        Check.That(File.ReadAllBytes(path2).Length).IsEqualTo(writtenByteLength);
    }

    [Fact]
    public void Should_Throw_If_Invalid_Params_Given()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        
        Check.ThatCode(() => new FileLogger(null)).Throws<ArgumentNullException>();
        
        Check.ThatCode(() => new FileLogger(string.Empty)).Throws<ArgumentException>();
        
        Check.ThatCode(() => new FileLogger(
            Path.GetTempFileName(),
            bufferSize: -1
        )).Throws<ArgumentOutOfRangeException>();
        
        Check.ThatCode(() => new FileLogger(
            Path.GetTempFileName(),
            bufferSize: 1000,
            capacity: -1
        )).Throws<ArgumentOutOfRangeException>();
        
        Check.ThatCode(() => new FileLogger(
            Path.GetTempFileName(),
            bufferSize: 1000,
            capacity: 1000,
            initialAllocationSize: -1
        )).Throws<ArgumentOutOfRangeException>();

#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    public void Dispose()
    {
        if (File.Exists(path)) File.Delete(path);
        if (File.Exists(path2)) File.Delete(path2);
    }    
}