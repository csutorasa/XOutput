using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using XOutput.Core.DependencyInjection;

namespace XOutput.Tools
{
    public sealed class ExternalRegistryModifierService : IRegistryModifierService
    {
        private readonly CommandRunner commandRunner;

        [ResolverMethod]
        public ExternalRegistryModifierService(CommandRunner commandRunner)
        {
            this.commandRunner = commandRunner;
        }

        public bool KeyExists(string key)
        {
            throw new NotImplementedException();
        }

        public void DeleteTree(string key)
        {
            commandRunner.StartPowershellAdmin($"Remove-Item -Path Registry::{key} -Recurse");
        }

        public void CreateKey(string key)
        {
            int splitter = key.LastIndexOf('\\');
            string basekey = key.Substring(0, splitter);
            string subkey = key.Substring(splitter + 1);
            commandRunner.StartPowershellAdmin($"New-Item -Path Registry::{basekey} -Name {subkey}");
        }

        public object GetValue(string key, string value)
        {
            throw new NotImplementedException();
        }

        public void SetValue(string key, string value, object newValue)
        {
            string newValueString;
            if (newValue is string)
            {
                newValueString = $"\"{newValue}\"";
            }
            else if (newValue is IEnumerable<string>)
            {
                var values = newValue as IEnumerable<string>;
                newValueString = $"@({ string.Join(",", values.Select(v => $"'{v}'")) })";
            }
            else
            {
                newValueString = newValue.ToString();
            }
            string command = $"Set-ItemProperty -Path Registry::{key} -Name \"{value}\" -Value { newValueString }";
            commandRunner.StartPowershellAdmin(command);
        }

        public void DeleteValue(string key, string value)
        {
            commandRunner.StartPowershellAdmin($"Remove-ItemProperty -Path Registry::{key} -Name \"{ value }\"");
        }
    }
}
