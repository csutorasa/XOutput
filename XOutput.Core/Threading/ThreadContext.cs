using System;
using System.Threading;

namespace XOutput.Core.Threading
{
    public class ThreadContext
    {
        private bool Stopped => thread == null || thread.ThreadState.HasFlag(ThreadState.Stopped) || thread.ThreadState.HasFlag(ThreadState.Aborted) || thread.ThreadState.HasFlag(ThreadState.Unstarted);

        private Thread thread;
        private CancellationTokenSource tokenSource;
        private ThreadResult result;

        internal ThreadContext(Thread thread, CancellationTokenSource tokenSource, ThreadResult result)
        {
            this.thread = thread;
            this.tokenSource = tokenSource;
            this.result = result;
        }

        public ThreadContext SetApartmentState(ApartmentState state)
        {
            thread.SetApartmentState(state);
            return this;
        }

        public ThreadContext Start()
        {
            if (Stopped)
            {
                thread.Start();
                return this;
            }
            throw new InvalidOperationException("Thread is already running!");
        }

        public ThreadContext Cancel()
        {
            if (!Stopped)
            {
                tokenSource.Cancel();
            }
            return this;
        }

        public ThreadResult Wait()
        {
            if (!Stopped)
            {
                thread.Join();
            }
            return result;
        }
    }
}
