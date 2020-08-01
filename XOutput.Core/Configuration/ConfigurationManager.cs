using System;
using System.IO;
using System.Text;

namespace XOutput.Core.Configuration
{
    public abstract class ConfigurationManager
    {
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
            using (StreamWriter writer = new StreamWriter(new FileStream(configuration.FilePath, FileMode.OpenOrCreate, FileAccess.Write), Encoding.UTF8))
            {
                WriteConfiguration(writer, configuration);
            }
        }

        public T Load<T>(string filePath, Func<T> defaultGetter) where T : ConfigurationBase
        {
            string path = GetFilePath(filePath);
            if (File.Exists(path))
            {
                using (StreamReader reader = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read), Encoding.UTF8))
                {
                    return ReadConfiguration<T>(reader);
                }
            }
            if (defaultGetter != null)
            {
                var defaultValue = defaultGetter();
                Save(path, defaultValue);
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
