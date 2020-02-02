using NLog;
using System;
using System.Threading;
using System.Threading.Tasks;

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
            var thread = new Thread(() => ThreadAction(name, threadResult, action, token));
            thread.Name = name;
            thread.IsBackground = isBackground;
            return new ThreadContext(thread, source, threadResult);
        }

        public static ThreadContext Create(string name, Func<CancellationToken, Task> action, bool isBackground = true)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            var threadResult = new ThreadResult();
            var thread = new Thread(() => ThreadAction(name, threadResult, (t) => action(t).Wait(), token));
            thread.Name = name;
            thread.IsBackground = isBackground;
            return new ThreadContext(thread, source, threadResult);
        }

        private static void ThreadAction(string name, ThreadResult threadResult, Action<CancellationToken> action, CancellationToken token)
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
        }
    }
}
