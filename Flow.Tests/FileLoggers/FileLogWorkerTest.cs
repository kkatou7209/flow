using System.Text;
using Flow.FileLoggers;

using SystemFile = System.IO.File;

namespace Flow.Tests.FileLoggers;

public sealed class FileLogWorkerTest : IDisposable
{
    private readonly string path = Path.GetTempFileName();

    private readonly string path2 = Path.GetTempFileName();

    [Fact]
    public async Task Should_Write_To_Log_File()
    {
        using (var worker = new FileLogWorker(
            path,
            bufferSize: 10_000,
            capacity: 10_000,
            initialAllocationSize: 10_000,
            encoding: Encoding.Default
        ))
        {
            await worker.SendAsync("Hello");
        }

        var log = SystemFile.ReadAllText(path);

        Check.That(log).IsEqualTo("Hello");
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

        using (var worker = new FileLogWorker(
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
                    tasks.Add(Task.Run(async () => await worker.SendAsync(MESSAGE)));
                }
            }

            await Task.WhenAll(tasks);
        };

        Check.That(File.ReadAllBytes(path2).Length).IsEqualTo(writtenByteLength);
    }

    public void Dispose()
    {
        if (SystemFile.Exists(path))
            SystemFile.Delete(path);

        if (SystemFile.Exists(path2))
            SystemFile.Delete(path2);
    }
}