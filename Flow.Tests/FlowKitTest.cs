using System.Text;
using Flow.FileLoggers;

namespace Flow.Tests;

public sealed class FlowKitTest : IDisposable
{
    public readonly string path = Path.GetTempFileName();

    [Fact]
    public void Should_Create_FileLogger()
    {
        var logger = FlowKit.ForFile(path)
            .WithAllocationSize(1000000)
            .WithBufferSize(64_000)
            .WithCapacity(30_000)
            .WithEncoding(new UTF32Encoding())
            .Build();

        Check.That(logger).IsInstanceOf<FileLogger>();
    }

    public void Dispose()
    {
        if (File.Exists(path)) File.Delete(path);
    }
}