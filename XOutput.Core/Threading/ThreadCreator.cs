using NLog;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace XOutput.Core.Threading
{
    public static class ThreadCreator
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();

        public static ThreadContext Create(string name, Action<CancellationToken> action, bool isBackground = true)
        {
            return DoCreate(name, action, isBackground);
        }

        public static ThreadContext Create(string name, Func<CancellationToken, Task> action, bool isBackground = true)
        {
            return DoCreate(name, (t) => action(t).Wait(), isBackground);
        }

        public static ThreadContext CreateLoop(string name, Action action, int delay, bool isBackground = true)
        {
            return DoCreate(name, Loop(action, delay), isBackground);
        }

        public static ThreadContext CreateLoop(string name, Func<Task> action, int delay, bool isBackground = true)
        {
            return DoCreate(name, Loop(() => action().Wait(), delay), isBackground);
        }

        private static ThreadContext DoCreate(string name, Action<CancellationToken> action, bool isBackground)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            var threadResult = new ThreadResult();
            var thread = new Thread(() => ThreadAction(name, threadResult, action, token));
            thread.Name = name;
            thread.IsBackground = isBackground;
            return new ThreadContext(thread, source, threadResult);
        }

        private static Action<CancellationToken> Loop(Action action, int delay)
        {
            return (token) =>
            {
                Stopwatch stopwatch = new Stopwatch();
                while (!token.IsCancellationRequested)
                {
                    stopwatch.Restart();
                    action();
                    stopwatch.Stop();
                    int remaining = delay - (int)stopwatch.ElapsedMilliseconds;
                    if (remaining < 0)
                    {
                        logger.Debug(() => $"Thread {Thread.CurrentThread.Name} loop is slower than expected by {-remaining} ms");
                        Thread.Sleep(0);
                    }
                    else
                    {
                        Thread.Sleep(remaining);
                    }

                }
            };
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
