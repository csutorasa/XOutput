using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XOutput.Core.DependencyInjection;

namespace XOutput.Tools
{
    public class FirewallService
    {
        private readonly CommandRunner commandRunner;

        [ResolverMethod]
        public FirewallService(CommandRunner commandRunner)
        {
            this.commandRunner = commandRunner;
        }

        public void AddException()
        {
            RefreshRule("XOutput", Assembly.GetExecutingAssembly().Location);
        }

        private void RefreshRule(string name, string file)
        {
            commandRunner.RunCmdAdmin($"netsh advfirewall firewall delete rule name=\"{name}\" & netsh advfirewall firewall add rule name=\"{name}\" dir=in action=allow program=\"{file}\" enable=yes");
        }
    }
}
