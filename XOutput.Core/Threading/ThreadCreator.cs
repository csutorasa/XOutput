using NLog;
using System;
using System.Threading;

namespace XOutput.Core.Threading
{
    public static class ThreadCreator
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();

        public static ThreadContext Create(string name, Action<CancellationToken> action, bool isBackground = true)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            var threadResult = new ThreadResult();
            var thread = new Thread(() =>
            {
                logger.Debug(() => $"Thread {name} is started.");
                try
                {
                    action.Invoke(token);
                }
                catch (Exception e)
                {
                    if (!token.IsCancellationRequested)
                    {
                        logger.Error(e, $"Thread {name} failed with error.");
                        threadResult.Error = e;
                    }
                }
                logger.Debug(() => $"Thread {name} is stopped.");
            });
            thread.Name = name;
            thread.IsBackground = isBackground;
            return new ThreadContext(thread, source, threadResult);
        }
    }
}
