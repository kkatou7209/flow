using System;
using System.IO;
using System.Text;

namespace Flow
{
    /// <summary>
    /// Entry of Flow library.
    /// </summary>
    public static class FlowKit
    {
        private static readonly Lazy<Logger> console
            = new Lazy<Logger>(() => new Logger(System.Console.Out, 50_000));

        /// <summary>
        /// Gets <c>Logger</c> for <c>System.Console</c>.
        /// </summary>
        public static Logger Console() => console.Value;

        /// <summary>
        /// Gets logger for <c>System.Console</c> with format.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formatter"></param>
        /// <returns></returns>
        public static FormatLogger<T> Console<T>(LogFormatter<T> formatter)
            => new FormatLogger<T>(Console(), formatter);

        /// <summary>
        /// Creates logger from <c>TextWriter</c>.
        /// </summary>
        /// <param name="writer"></param>
        public static Logger Create(TextWriter writer, int capacity = 30_000)
            => new Logger(writer, capacity);

        /// <summary>
        /// Creates logger from <c>Stream</c>.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="encoding"></param>
        /// <param name="capacity"></param>
        public static Logger Create(Stream stream, Encoding encoding = null, int capacity = 30_000)
            => new Logger(stream, encoding ?? Encoding.Default, capacity);

        /// <summary>
        /// Creates logger for specified file.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bufferSize"></param>
        /// <param name="encoding"></param>
        /// <param name="capacity"></param>
        /// <returns></returns>
        public static Logger Create(string path, int bufferSize = 64_000, Encoding encoding = null, int capacity = 30_000)
            => new Logger(path, encoding, bufferSize, capacity);

        /// <summary>
        /// Create logger with format.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formatter"></param>
        /// <param name="writer"></param>
        /// <param name="capacity"></param>
        /// <returns></returns>
        public static FormatLogger<T> Create<T>(
            LogFormatter<T> formatter,
            TextWriter writer, int capacity = 30_000
        ) => new FormatLogger<T>(
            Create(writer, capacity),
            formatter
        );

        /// <summary>
        /// Create logger with format.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formatter"></param>
        /// <param name="stream"></param>
        /// <param name="encoding"></param>
        /// <param name="capacity"></param>
        /// <returns></returns>
        public static FormatLogger<T> Create<T>(
            LogFormatter<T> formatter,
            Stream stream, Encoding encoding = null, int capacity = 30_000
        ) => new FormatLogger<T>(
            Create(stream, encoding, capacity),
            formatter
        );

        /// <summary>
        /// Create logger with format.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formatter"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static FormatLogger<T> Create<T>(
            LogFormatter<T> formatter,
            Logger logger
        ) => new FormatLogger<T>(logger, formatter);

        /// <summary>
        /// Creates logger for specified file with format.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formatter"></param>
        /// <param name="path"></param>
        /// <param name="bufferSize"></param>
        /// <param name="encoding"></param>
        /// <param name="capacity"></param>
        /// <returns></returns>
        public static FormatLogger<T> Create<T>(
            LogFormatter<T> formatter,
            string path, Encoding encoding = null, int bufferSize = 64_000, int capacity = 30_000
        ) => new FormatLogger<T>(
            Create(path, bufferSize, encoding, capacity),
            formatter
        );

        /// <summary>
        /// Combines loggers and create new logger.
        /// </summary>
        /// <param name="loggers"></param>
        public static FanOutLogger Combine(params Logger[] loggers)
            => new FanOutLogger(loggers);

        /// <summary>
        /// Combines loggers with formats and create new logger.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loggers"></param>
        /// <returns></returns>
        public static FormatFanOutLogger<T> Combine<T>(params FormatLogger<T>[] loggers)
            => new FormatFanOutLogger<T>(loggers);
    }
}