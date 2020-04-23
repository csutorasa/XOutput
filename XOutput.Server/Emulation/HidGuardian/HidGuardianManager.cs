using Microsoft.Win32;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using XOutput.Core.Configuration;
using XOutput.Core.DependencyInjection;

namespace XOutput.Server.Emulation.HidGuardian
{
    public class HidGuardianManager : IDisposable
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        static readonly string PARAMETERS = Registry.LocalMachine.ToString() + "\\SYSTEM\\CurrentControlSet\\Services\\HidGuardian\\Parameters";
        static readonly string WHITE_LIST = PARAMETERS + "\\Whitelist";
        private const string AFFECTED_DEVICES = "AffectedDevices";

        public bool Installed { get; private set; }

        private readonly RegistryModifierService registryModifierService;
        private bool disposed;

        [ResolverMethod]
        public HidGuardianManager(RegistryModifierService registryModifierService)
        {
            this.registryModifierService = registryModifierService;
            string driverFile = Path.Combine(Environment.SystemDirectory, "drivers", "HidGuardian.sys");
            if (File.Exists(driverFile)) {
                Installed = true;
                logger.Info($"HidGuardian is installed with version: {FileVersionInfo.GetVersionInfo(driverFile).FileVersion}");
            }
            else
            {
                Installed = false;
                logger.Info($"HidGuardian is not installed");
            }
        }

        public void ResetPid(int pid)
        {
            ClearPids();
            SetPid(pid);
        }

        public void SetPid(int pid)
        {
            registryModifierService.CreateKey(WHITE_LIST + "\\" + pid);
        }

        public void ClearPid(int pid)
        {
            registryModifierService.DeleteTree(WHITE_LIST + "\\" + pid);
        }

        public void ClearPids()
        {
            registryModifierService.DeleteTree(WHITE_LIST);
        }

        public List<string> GetDevices()
        {
            object value = registryModifierService.GetValue(PARAMETERS, AFFECTED_DEVICES);
            if (value is string[])
            {
                return new List<string>(value as string[]);
            }
            return new List<string>();
        }

        public void AddAffectedDevice(string device)
        {
            if (device == null)
            {
                return;
            }
            var devices = GetDevices();
            devices.Add(device);
            registryModifierService.SetValue(PARAMETERS, AFFECTED_DEVICES, devices.ToArray());
        }

        public bool RemoveAffectedDevice(string device)
        {
            if (device == null)
            {
                return false;
            }
            var devices = GetDevices();
            bool removed = devices.Remove(device);
            if (removed)
            {
                registryModifierService.SetValue(PARAMETERS, AFFECTED_DEVICES, devices.ToArray());
            }
            return removed;
        }

        public bool IsAffected(string device)
        {
            var devices = GetDevices();
            return devices.Contains(device);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            if (disposing)
            {
                ClearPid(Process.GetCurrentProcess().Id);
            }
            disposed = true;
        }
    }
}
