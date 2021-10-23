using NLog;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace XOutput.External
{
    public class CommandRunner
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();

        public Process CreatePowershell(string command)
        {
            string escapedCommand = GetEscapedCommand(command);
            var startInfo = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = "-Command \"" + escapedCommand + "\"",
                CreateNoWindow = true,
                UseShellExecute = false,
            };
            return new Process
            {
                StartInfo = startInfo,
            };
        }

        public void RunProcess(Process process)
        {
            process.Start();
            WaitForExit(process);
        }

        public Task RunProcessAsync(Process process, CancellationToken? token = null)
        {
            process.Start();
            token?.Register(() =>
            {
                process.Kill();
            });
            return Task.Run(() =>
            {
                WaitForExit(process);
            });
        }

        private void WaitForExit(Process process)
        {
            logger.Info("{0} {1} is started", process.StartInfo.FileName, process.StartInfo.Arguments);
            process.WaitForExit();
            logger.Info("{0} {1} is exited with code {2}", process.StartInfo.FileName, process.StartInfo.Arguments, process.ExitCode);
        }

        private string GetEscapedCommand(string command)
        {
            return command.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}
