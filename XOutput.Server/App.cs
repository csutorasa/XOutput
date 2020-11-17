using NLog;
using NLog.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using XOutput.Core;
using XOutput.Core.Configuration;
using XOutput.Core.DependencyInjection;
using XOutput.Core.Notifications;
using XOutput.Core.Resources;
using XOutput.Core.Versioning;

namespace XOutput.App
{
    public class App
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly string appVersion;


        public static void Main()
        {
            string exePath = Assembly.GetExecutingAssembly().Location;
            string cwd = Path.GetDirectoryName(exePath);
            Directory.SetCurrentDirectory(cwd);
            new App(Assembly.GetExecutingAssembly().GetName().Version.ToString(3)).Run().Wait();
        }

        public App(string appVersion)
        {
            this.appVersion = appVersion;
        }

        private async Task Run()
        {
            SetLoggerConfiguration();

            // AppDomain.CurrentDomain.FirstChanceException += (object sender, FirstChanceExceptionEventArgs e) => UnhandledException(e.Exception, LogLevel.Info);
            AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) => UnhandledException(e.ExceptionObject as Exception, LogLevel.Error);
            TaskScheduler.UnobservedTaskException += (object sender, UnobservedTaskExceptionEventArgs e) => UnhandledException(e.Exception, LogLevel.Error);

            logger.Info($"Starting XOutput server version: {appVersion}");

            var globalContext = ApplicationContext.Global;
            globalContext.AddFromConfiguration(typeof(CoreConfiguration));
            globalContext.AddFromConfiguration(typeof(ServerConfiguration));
            globalContext.Discover(GetOrLoadAssemblies("XOutput.Core", "XOutput.Api", "XOutput.Devices", "XOutput.Emulation", "XOutput.Server"));
            logger.Info("Configuration classes are loaded");
            var configurationManager = globalContext.Resolve<ConfigurationManager>();

            var notificationService = globalContext.Resolve<NotificationService>();

            await CheckUpdate(globalContext.Resolve<UpdateChecker>(), notificationService);

            var server = globalContext.Resolve<Server.Server>();
            server.Run();
            globalContext.Close();
        }

        private void SetLoggerConfiguration()
        {
            try
            {
                if (File.Exists("nlog.config"))
                {
                    LogManager.Configuration = new XmlLoggingConfiguration(XmlReader.Create(File.OpenRead("nlog.config")));
                    return;
                }
            }
            catch
            {
                // Cannot create logger
            }
            LogManager.Configuration = new XmlLoggingConfiguration(XmlReader.Create(AssemblyResourceManager.GetResourceStream("XOutput.Server.nlog.config")));
        }

        private Task CheckUpdate(UpdateChecker updateChecker, NotificationService notificationService)
        {
            return updateChecker.CompareRelease(appVersion).ContinueWith(t => {
                switch (t.Result.Result)
                {
                    case VersionCompareValues.NeedsUpgrade:
                        notificationService.Add(Notifications.NeedsVersionUpgrade, new List<string>() { t.Result.LatestVersion });
                        break;
                    case VersionCompareValues.Error:
                        notificationService.Add(Notifications.VersionCheckError, new List<string>() { }, NotificationTypes.Warning);
                        break;
                }
            });
        }

        public static void UnhandledException(Exception exceptionObject, LogLevel level)
        {
            logger.Log(level, exceptionObject);
        }

        private IEnumerable<Assembly> GetOrLoadAssemblies(params string[] assemblyNames)
        {
            return assemblyNames.Select(assemblyName =>
            {
                var assembly = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName == assemblyName).FirstOrDefault();
                if (assembly != null)
                {
                    return assembly;
                }
                return AppDomain.CurrentDomain.Load(assemblyName);
            }).ToList();
        }
    }
}