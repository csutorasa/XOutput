using System;
using System.IO;

namespace XOutput.Core.Configuration
{
    public abstract class ConfigurationManager
    {
        public void Save<T>(string filePath, T configuration) where T : IConfiguration
        {
            File.WriteAllText(filePath, ConfigurationToString(configuration));
        }

        public T Load<T>(string filePath, Func<T> defaultGetter) where T : IConfiguration
        {
            if (File.Exists(filePath))
            {
                var text = File.ReadAllText(filePath);
                return StringToConfiguration<T>(text);
            }
            if (defaultGetter != null)
            {
                return defaultGetter();
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

        protected abstract string ConfigurationToString<T>(T configuration) where T : IConfiguration;
        protected abstract T StringToConfiguration<T>(string configuration) where T : IConfiguration;
    }
}
