using System;

namespace XOutput.Logging
{
    public interface ILogger
    {
        /// <summary>
        /// Writes a trace log.
        /// </summary>
        /// <param name="log">log message</param>
        /// <returns></returns>
        void Trace(string log);
        /// <summary>
        /// Writes a trace log with lazy evaluation.
        /// </summary>
        /// <param name="log">log message generator</param>
        /// <returns></returns>
        void Trace(Func<string> log);
        /// <summary>
        /// Writes a debug log.
        /// </summary>
        /// <param name="log">log message</param>
        /// <returns></returns>
        void Debug(string log);
        /// <summary>
        /// Writes a debug log with lazy evaluation.
        /// </summary>
        /// <param name="log">log message generator</param>
        /// <returns></returns>
        void Debug(Func<string> log);
        /// <summary>
        /// Writes a info log.
        /// </summary>
        /// <param name="log">log message</param>
        /// <returns></returns>
        void Info(string log);
        /// <summary>
        /// Writes a info log with lazy evaluation.
        /// </summary>
        /// <param name="log">log message generator</param>
        /// <returns></returns>
        void Info(Func<string> log);
        /// <summary>
        /// Writes a warning log.
        /// </summary>
        /// <param name="log">log message</param>
        /// <returns></returns>
        void Warning(string log);
        /// <summary>
        /// Writes a warning log with lazy evaluation.
        /// </summary>
        /// <param name="log">log message generator</param>
        /// <returns></returns>
        void Warning(Func<string> log);
        /// <summary>
        /// Writes a warning log.
        /// </summary>
        /// <param name="ex">exception</param>
        /// <returns></returns>
        void Warning(Exception ex);
        /// <summary>
        /// Writes a warning log.
        /// </summary>
        /// <param name="log">log message</param>
        /// <param name="ex">exception</param>
        /// <returns></returns>
        void Warning(string log, Exception ex);
        /// <summary>
        /// Writes a warning log with lazy evaluation.
        /// </summary>
        /// <param name="log">log message generator</param>
        /// <param name="ex">exception</param>
        /// <returns></returns>
        void Warning(Func<string> log, Exception ex);
        /// <summary>
        /// Writes a error log.
        /// </summary>
        /// <param name="log">log message</param>
        /// <returns></returns>
        void Error(string log);
        /// <summary>
        /// Writes a error log with lazy evaluation.
        /// </summary>
        /// <param name="log">log message generator</param>
        /// <returns></returns>
        void Error(Func<string> log);
        /// <summary>
        /// Writes a error log.
        /// </summary>
        /// <param name="ex">exception</param>
        /// <returns></returns>
        void Error(Exception ex);
        /// <summary>
        /// Writes a error log.
        /// </summary>
        /// <param name="log">log message</param>
        /// <param name="ex">exception</param>
        /// <returns></returns>
        void Error(string log, Exception ex);
        /// <summary>
        /// Writes a error log with lazy evaluation.
        /// </summary>
        /// <param name="log">log message generator</param>
        /// <param name="ex">exception</param>
        /// <returns></returns>
        void Error(Func<string> log, Exception ex);

        void SafeCall(Action action);
        void SafeCall(Action action, string log);
        void SafeCall(Action action, string log, LogLevel level);
        T SafeCall<T>(Func<T> action);
        T SafeCall<T>(Func<T> action, string log);
        T SafeCall<T>(Func<T> action, string log, LogLevel level);
    }
}
