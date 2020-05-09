using System.Collections.Generic;

namespace XOutput.Api.Notifications
{
    public class Notification
    {
        public const string Information = "Info";
        public const string Warning = "Warn";
        public const string Error = "Error";

        public string Key { get; set; }
        public string Level { get; set; }
        public List<string> Parameters { get; set; }
    }
}
