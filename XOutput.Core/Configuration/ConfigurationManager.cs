using System;
using System.IO;
using System.Text;
using NLog;

namespace XOutput.Core.Configuration
{
    public abstract class ConfigurationManager
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public void Save<T>(string filePath, T configuration) where T : ConfigurationBase
        {
            configuration.FilePath = GetFilePath(filePath);
            Save(configuration);
        }

        public void Save<T>(T configuration) where T : ConfigurationBase
        {
            string directory = Path.GetDirectoryName(configuration.FilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            using (StreamWriter writer = new StreamWriter(new FileStream(configuration.FilePath, FileMode.Create, FileAccess.Write), Encoding.UTF8))
            {
                WriteConfiguration(writer, configuration);
                logger.Info($"Configuration saved to {configuration.FilePath}");
            }
        }

        public T Load<T>(string filePath, Func<T> defaultGetter) where T : ConfigurationBase
        {
            string path = GetFilePath(filePath);
            if (File.Exists(path))
            {
                using (StreamReader reader = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read), Encoding.UTF8))
                {
                    var config = ReadConfiguration<T>(reader);
                    logger.Info($"Configuration loaded from {path}");
                    return config;
                }
            }
            if (defaultGetter != null)
            {
                var defaultValue = defaultGetter();
                Save(filePath, defaultValue);
                return defaultValue;
            }
            return default;
        }

        public static IDisposable Watch(string filePath, Action<string> handler)
        {
            return Watch(Path.GetDirectoryName(filePath), Path.GetFileName(filePath), handler);
        }

        public static IDisposable Watch(string directory, string filter, Action<string> handler)
        {
            FileSystemWatcher watcher = new FileSystemWatcher
            {
                Path = directory,
                Filter = filter,
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
            };

            watcher.Changed += (sender, args) => handler.Invoke(args.FullPath);
            watcher.Created += (sender, args) => handler.Invoke(args.FullPath);
            watcher.Deleted += (sender, args) => handler.Invoke(args.FullPath);
            watcher.Renamed += (sender, args) => { handler.Invoke(args.OldFullPath); handler.Invoke(args.OldFullPath); };

            watcher.EnableRaisingEvents = true;
            return watcher;
        }

        protected abstract void WriteConfiguration<T>(StreamWriter writer, T configuration) where T : ConfigurationBase;
        protected abstract T ReadConfiguration<T>(StreamReader reader) where T : ConfigurationBase;
        protected abstract string GetFilePath(string path);
    }
}
