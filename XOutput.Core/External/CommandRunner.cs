using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace XOutput.Core.External
{
    public class CommandRunner
    {
        public Task<Process> StartCmdAsync(string command, CancellationToken? token = null)
        {
            return RunProcessAsync("cmd.exe", $"/C {command}", token);
        }

        public string RunCmd(string command)
        {
            var task = RunProcessAsync("cmd.exe", $"/C {command}");
            task.Wait();
            var process = task.Result;
            if (0 == process.ExitCode)
            {
                return process.StandardOutput.ReadToEnd();
            }
            else
            {
                throw new ProcessErrorException(process);
            }
        }

        private Task<Process> RunProcessAsync(string filename, string arguments = null, CancellationToken? token = null)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = filename,
                Arguments = arguments,
                CreateNoWindow = true,
                UseShellExecute = false,
            };
            var process = new Process
            {
                StartInfo = startInfo,
            };
            process.Start();
            token?.Register(() =>
            {
                process.Kill();
            });
            return Task.Run(() =>
            {
                process.WaitForExit();
                return process;
            });
        }
    }
}
