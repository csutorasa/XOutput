using System;
using System.IO;
using System.Text;
using NLog;

namespace XOutput.Configuration
{
    public abstract class ConfigurationManager
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public void Save<T>(string filePath, T configuration) where T : ConfigurationBase
        {
            configuration.FilePath = filePath;
            Save(configuration);
        }

        public void Save<T>(T configuration) where T : ConfigurationBase
        {
            string pathWithExtension = GetFilePath(configuration.FilePath);
            string directory = Path.GetDirectoryName(pathWithExtension);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            using (StreamWriter writer = new StreamWriter(new FileStream(pathWithExtension, FileMode.Create, FileAccess.Write), Encoding.UTF8))
            {
                WriteConfiguration(writer, configuration);
                logger.Info($"Configuration saved to {pathWithExtension}");
            }
        }

        public T Load<T>(Func<T> defaultGetter) where T : ConfigurationBase
        {
            return Load<T>(GetConfigurationPathFromAttribute<T>(), defaultGetter);
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
                    config.FilePath = filePath;
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

        private static string GetConfigurationPathFromAttribute<T>() where T : ConfigurationBase
        {
            foreach (var attribute in typeof(T).GetCustomAttributes(false))
            {
                if (attribute is ConfigurationPathAttribute)
                {
                    return (attribute as ConfigurationPathAttribute).Path;
                }
            }
            throw new ArgumentException($"{typeof(T).FullName} does not have {nameof(ConfigurationPathAttribute)} attribute");
        }

        protected abstract void WriteConfiguration<T>(StreamWriter writer, T configuration) where T : ConfigurationBase;
        protected abstract T ReadConfiguration<T>(StreamReader reader) where T : ConfigurationBase;
        protected abstract string GetFilePath(string path);
    }
}
