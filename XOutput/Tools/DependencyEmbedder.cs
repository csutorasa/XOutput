using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using XOutput.Logging;

namespace XOutput.Tools
{
    public class DependencyEmbedder
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(DependencyEmbedder));

        private static DependencyEmbedder instance = new DependencyEmbedder();
        /// <summary>
        /// Gets the singleton instance of the class.
        /// </summary>
        public static DependencyEmbedder Instance => instance;
        private List<KeyValuePair<string, string>> packages = new List<KeyValuePair<string, string>>();

        public void AddPackage(string package)
        {
            AddPackage(package, package);
        }
        public void AddPackage(string package, string dllFile)
        {
            packages.Add(new KeyValuePair<string, string>(package, dllFile));
        }

        public void Initialize()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            foreach (var package in packages)
            {
                if (args.Name.StartsWith(package.Key))
                {
                    logger.Info("Loading " + package.Value + ".dll from embedded resources");
                    return LoadAssemblyFromResource(Assembly.GetExecutingAssembly().GetName().Name + "." + package.Value + ".dll");
                }
            }
            return null;
        }

        private Assembly LoadAssemblyFromResource(string resourceName)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                byte[] assemblyData = new byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
        }
    }
}
