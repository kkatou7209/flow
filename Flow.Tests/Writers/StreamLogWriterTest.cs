namespace Flow.Tests.Writers;

using System.Text;
using Flow.Writers;

public sealed class StreamLogWriterTest
{
    [Fact]
    public async Task Should_Write_Log_To_Stream()
    {
        using var memory = new MemoryStream();

        using var writer = new StreamLogWriter(memory, Encoding.UTF8);

        writer.Write($"[13:35:43] [INFO] Testing あいうえお{Environment.NewLine}");

        await writer.WriteAsync($"[13:35:50] [ERROR] Testing あかさたな{Environment.NewLine}");

        using var reader = new StreamReader(memory, Encoding.UTF8);

        memory.Position = 0;

        var log = reader.ReadToEnd();

        Check.That(log)
            .IsEqualTo(
                """
                [13:35:43] [INFO] Testing あいうえお
                [13:35:50] [ERROR] Testing あかさたな

                """
            );
    }

    [Fact]
    public async Task Should_Write_Logs_To_File()
    {
        string path = Path.GetTempFileName();

        try
        {
            using var file = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read);

            using var writer = new StreamLogWriter(file, Encoding.UTF8);

            writer.Write($"[13:35:43] [INFO] Testing あいうえお{Environment.NewLine}");

            await writer.WriteAsync($"[13:35:50] [ERROR] Testing あかさたな{Environment.NewLine}");

            var log = File.ReadAllText(path, Encoding.UTF8);

            Check.That(log)
                .IsEqualTo(
                    """
                    [13:35:43] [INFO] Testing あいうえお
                    [13:35:50] [ERROR] Testing あかさたな

                    """
                );
        }
        finally
        {
            File.Delete(path);
        }
    }
}