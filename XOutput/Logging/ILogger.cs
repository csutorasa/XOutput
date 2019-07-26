using System;
using System.Threading.Tasks;

namespace XOutput.Logging
{
    public interface ILogger
    {
        /// <summary>
        /// Writes a trace log.
        /// </summary>
        /// <param name="log">log message</param>
        /// <returns></returns>
        Task Trace(string log);
        /// <summary>
        /// Writes a trace log with lazy evaluation.
        /// </summary>
        /// <param name="log">log message generator</param>
        /// <returns></returns>
        Task Trace(Func<string> log);
        /// <summary>
        /// Writes a debug log.
        /// </summary>
        /// <param name="log">log message</param>
        /// <returns></returns>
        Task Debug(string log);
        /// <summary>
        /// Writes a debug log with lazy evaluation.
        /// </summary>
        /// <param name="log">log message generator</param>
        /// <returns></returns>
        Task Debug(Func<string> log);
        /// <summary>
        /// Writes a info log.
        /// </summary>
        /// <param name="log">log message</param>
        /// <returns></returns>
        Task Info(string log);
        /// <summary>
        /// Writes a info log with lazy evaluation.
        /// </summary>
        /// <param name="log">log message generator</param>
        /// <returns></returns>
        Task Info(Func<string> log);
        /// <summary>
        /// Writes a warning log.
        /// </summary>
        /// <param name="log">log message</param>
        /// <returns></returns>
        Task Warning(string log);
        /// <summary>
        /// Writes a warning log with lazy evaluation.
        /// </summary>
        /// <param name="log">log message generator</param>
        /// <returns></returns>
        Task Warning(Func<string> log);
        /// <summary>
        /// Writes a warning log.
        /// </summary>
        /// <param name="ex">exception</param>
        /// <returns></returns>
        Task Warning(Exception ex);
        /// <summary>
        /// Writes a error log.
        /// </summary>
        /// <param name="log">log message</param>
        /// <returns></returns>
        Task Error(string log);
        /// <summary>
        /// Writes a error log with lazy evaluation.
        /// </summary>
        /// <param name="log">log message generator</param>
        /// <returns></returns>
        Task Error(Func<string> log);
        /// <summary>
        /// Writes a error log.
        /// </summary>
        /// <param name="ex">exception</param>
        /// <returns></returns>
        Task Error(Exception ex);
    }
}
