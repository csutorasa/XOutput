using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace XOutput.Tools
{
    public static class DispatcherHelper
    {
        public static void InvokeIfNeeded(Action task)
        {
            Application.Current.Dispatcher.InvokeIfNeeded(task);
        }
        public static void InvokeIfNeeded(this Dispatcher dispatcher, Action task)
        {
            if (dispatcher.Thread != Thread.CurrentThread)
            {
                dispatcher.Invoke(task);
            }
            else
            {
                task();
            }
        }
    }
}
