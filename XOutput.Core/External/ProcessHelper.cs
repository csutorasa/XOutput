using System.Diagnostics;

namespace XOutput.External
{
    public static class ProcessHelper
    {
        public static Process Parent(this Process process)
        {
            /*var pid = process.Id;
            var processName = Process.GetProcessById(pid).ProcessName;
            var processesByName = Process.GetProcessesByName(processName);
            string processIndexedName = null;

            for (var index = 0; index < processesByName.Length; index++) {
                var name = index == 0 ? processName : processName + "#" + index;
                var processId = new PerformanceCounter("Process", "ID Process", name);
                if ((int) processId.NextValue() == pid) {
                    processIndexedName = name;
                    break;
                }
            }
            
            if (processIndexedName != null) {
                var parentId = new PerformanceCounter("Process", "Creating Process ID", processIndexedName);
                return Process.GetProcessById((int) parentId.NextValue());
            }*/
            return null;
        }
    }
}