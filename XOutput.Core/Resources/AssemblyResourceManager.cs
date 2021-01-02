using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace XOutput.Core.Resources
{
    public static class AssemblyResourceManager
    {
        public static Stream GetResourceStream(string resource)
        {
            return GetResourceStream(resource, AppDomain.CurrentDomain.GetAssemblies());
        }

        public static string GetResourceString(string resource)
        {
            return GetResourceString(resource, Encoding.UTF8, AppDomain.CurrentDomain.GetAssemblies());

        }

        public static string GetResourceString(string resource, Encoding encoding, params Assembly[] assemblies)
        {
            using (var stream = new StreamReader(GetResourceStream(resource, assemblies), encoding))
            {
                return stream.ReadToEnd();
            }
        }

        public static Stream GetResourceStream(string resource, params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                var stream = assembly.GetManifestResourceStream(resource);
                if (stream != null)
                {
                    return stream;
                }
            }
            throw new ArgumentException($"Cannot find resource {resource}");
        }
    }
}
