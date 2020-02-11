using System;
using System.IO;
using System.Text;

namespace XOutput.Core.Configuration
{
    public abstract class ConfigurationManager
    {
        public void Save<T>(string filePath, T configuration) where T : IConfiguration
        {
            using (StreamWriter writer = new StreamWriter(new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write), Encoding.UTF8)) {
                WriteConfiguration(writer, configuration);
            }
        }

        public T Load<T>(string filePath, Func<T> defaultGetter) where T : IConfiguration
        {
            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(new FileStream(filePath, FileMode.Open, FileAccess.Read), Encoding.UTF8)) {
                    return ReadConfiguration<T>(reader);
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

        public IDisposable Watch(string filePath, Action<string> handler)
        {
            return Watch(Path.GetDirectoryName(filePath), Path.GetFileName(filePath), handler);
        }

        public IDisposable Watch(string directory, string filter, Action<string> handler)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = directory;
            watcher.Filter = filter;

            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            watcher.Changed += (sender, args) => handler.Invoke(args.FullPath);
            watcher.Created += (sender, args) => handler.Invoke(args.FullPath);
            watcher.Deleted += (sender, args) => handler.Invoke(args.FullPath);
            watcher.Renamed += (sender, args) => { handler.Invoke(args.OldFullPath); handler.Invoke(args.OldFullPath); };

            watcher.EnableRaisingEvents = true;
            return watcher;
        }

        protected abstract void WriteConfiguration<T>(StreamWriter writer, T configuration) where T : IConfiguration;
        protected abstract T ReadConfiguration<T>(StreamReader reader) where T : IConfiguration;
    }
}
