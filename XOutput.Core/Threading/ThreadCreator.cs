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

        public static ThreadContext Create(string name, Action<CancellationToken> action, bool isBackground = true, CancellationToken token = default)
        {
            return DoCreate(name, action, isBackground, token);
        }

        public static ThreadContext Create(string name, Func<CancellationToken, Task> action, bool isBackground = true, CancellationToken token = default)
        {
            return DoCreate(name, (t) => action(t).Wait(), isBackground, token);
        }

        public static ThreadContext CreateLoop(string name, Action action, int delay, bool isBackground = true, CancellationToken token = default)
        {
            Action<CancellationToken> handler = (token) => action();
            var loop = delay <= 0 ? Loop(handler) : Loop(handler, delay);
            return DoCreate(name, loop, isBackground, token);
        }

        public static ThreadContext CreateLoop(string name, Action<CancellationToken> action, int delay, bool isBackground = true, CancellationToken token = default)
        {
            var loop = delay <= 0 ? Loop(action) : Loop(action, delay);
            return DoCreate(name, loop, isBackground, token);
        }

        private static ThreadContext DoCreate(string name, Action<CancellationToken> action, bool isBackground, CancellationToken token = default)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationTokenSource linkedTokens = CancellationTokenSource.CreateLinkedTokenSource(source.Token, token);
            var threadResult = new ThreadResult();
            var thread = new Thread(() => ThreadAction(name, threadResult, action, linkedTokens.Token));
            thread.Name = name;
            thread.IsBackground = isBackground;
            return new ThreadContext(thread, source, threadResult);
        }

        private static Action<CancellationToken> Loop(Action<CancellationToken> action, int delay)
        {
            return (token) =>
            {
                Stopwatch stopwatch = new Stopwatch();
                while (!token.IsCancellationRequested)
                {
                    stopwatch.Restart();
                    action(token);
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

        private static Action<CancellationToken> Loop(Action<CancellationToken> action)
        {
            return (token) =>
            {
                while (!token.IsCancellationRequested)
                {
                    action(token);
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
