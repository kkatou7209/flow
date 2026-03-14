using System.Threading.Tasks;

namespace Flow
{
    /// <summary>
    /// Logging functionality of Flow.
    /// </summary>
    public interface ILogging
    {
        /// <summary>
        /// Logs message.
        /// </summary>
        /// <param name="log"></param>
        void Log(string log);

        /// <summary>
        /// Logs message asynchronously
        /// </summary>
        /// <param name="log"></param>
        Task LogAsync(string log);
    }

    public interface ILogging<T>
    {
        /// <summary>
        /// Logs with specified type.
        /// </summary>
        /// <param name="log"></param>
        void Log(T log);

        /// <summary>
        /// Logs with specidied type asynchronousely.
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        Task LogAsync(T log);
    }
}