using System.Text;
using NFluent;

namespace Flow.Tests.Unit
{
    public sealed class FanOutLoggerTest : IDisposable
    {
        private readonly string[] paths = [.. Enumerable.Range(1, 200).Select(_ => Path.GetTempFileName())];

        [Fact(Timeout = 3000)]
        public async Task Should_Log_Multiple_Files()
        {
            var loggers = new List<Logger>(paths.Length);

            foreach(var path in paths)
            {
                loggers.Add(new Logger(path, Encoding.Default, 4069, 300));
            }

            using var logger = new FanOutLogger([.. loggers]);

            await logger.LogAsync("[15:48:29] GET /user/1 HTTP/2");

            logger.Dispose();

            const string expected = """
            [15:48:29] GET /user/1 HTTP/2

            """;

            foreach(var path in paths)
            {
                var log = File.ReadAllText(path);

                Check.That(log).IsEqualTo(expected);
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
}