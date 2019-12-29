using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XOutput.Logging;

namespace XOutput.Tools
{
    public static class ThreadHelper
    {
        private static ILogger logger = LoggerFactory.GetLogger(typeof(ThreadHelper));

        public static Thread Create(ThreadStartParameters parameters)
        {
            if (parameters.Task == null)
            {
                throw new ArgumentException(nameof(parameters));
            }
            var thread = new Thread(() =>
            {
                logger.Debug(() => $"Thread {parameters.Name} is started.");
                try
                {
                    parameters.Task();
                }
                catch (ThreadInterruptedException)
                {
                    logger.Debug(() => $"Thread {parameters.Name} is interrupted.");
                    throw;
                }
                catch (Exception e)
                {
                    logger.Error($"Thread {parameters.Name} failed with error.", e);
                    parameters?.Error(e);
                }
                finally
                {
                    parameters?.Finally();
                }
                logger.Debug(() => $"Thread {parameters.Name} is stopped.");
            });
            if (parameters.Name != null) {
                thread.Name = parameters.Name;
            }
            thread.IsBackground = parameters.IsBackground;
            return thread;
        }

        public static Thread CreateAndStart(ThreadStartParameters parameters)
        {
            Thread thread = Create(parameters);
            thread.Start();
            return thread;
        }

        public static void StopAndWait(Thread thread)
        {
            thread.Interrupt();
            thread.Join();
        }
    }

    public class ThreadStartParameters
    {
        public string Name { get; set; }
        public bool IsBackground { get; set; }
        public Action Task { get; set; }
        public Action Finally { get; set; }
        public Action<Exception> Error { get; set; }
    }
}
