using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Logging
{
    public class LogLevel
    {
        public static readonly LogLevel Trace = new LogLevel("TRACE", 20);
        public static readonly LogLevel Debug = new LogLevel("DEBUG", 40);
        public static readonly LogLevel Info = new LogLevel("INFO ", 60);
        public static readonly LogLevel Warning = new LogLevel("WARN ", 80);
        public static readonly LogLevel Error = new LogLevel("ERROR", 100);

        protected string text;
        public string Text => text;
        protected int level;
        public int Level => level;

        protected LogLevel(String text, int level)
        {
            this.text = text;
            this.level = level;
        }
    }
}
