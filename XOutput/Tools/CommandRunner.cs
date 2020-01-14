using System;
using System.Diagnostics;
using XOutput.Core.DependencyInjection;

namespace XOutput.Tools
{
    public class CommandRunner
    {
        [ResolverMethod]
        public CommandRunner()
        {
            
        }

        public Process StartCmdAsync(string command)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/C " + command,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                },
            };
            process.Start();
            return process;
        }

        public string RunCmd(string command)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/C " + command,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                },
            };
            process.Start();
            process.WaitForExit();
            if (0 == process.ExitCode)
            {
                return process.StandardOutput.ReadToEnd();
            }
            else
            {
                throw new Exception($"Process exited with {process.ExitCode}");
            }
        }

        public void RunCmdAdmin(string command)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/C " + command,
                    CreateNoWindow = true,
                    UseShellExecute = true,
                    Verb = "runas",
                },
            };
            process.Start();
            process.WaitForExit();
            if (0 != process.ExitCode)
            {
                throw new Exception($"Process exited with {process.ExitCode}");
            }
        }

        public void StartPowershellAdmin(string command)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = "-Command \"" + command + "\"",
                    CreateNoWindow = true,
                    UseShellExecute = true,
                    Verb = "runas",
                },
            };
            process.Start();
            process.WaitForExit();
            if (0 != process.ExitCode)
            {
                throw new Exception($"Process exited with {process.ExitCode}");
            }
        }
    }
}
