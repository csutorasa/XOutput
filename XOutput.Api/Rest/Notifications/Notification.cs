using System.Collections.Generic;

namespace XOutput.Rest.Notifications
{
    public class Notification
    {
        public const string Information = "Info";
        public const string Warning = "Warn";
        public const string Error = "Error";

        public string Id { get; set; }
        public bool Acknowledged { get; set; }
        public string CreatedAt { get; set; }
        public string Key { get; set; }
        public string Level { get; set; }
        public List<string> Parameters { get; set; }
    }
}
