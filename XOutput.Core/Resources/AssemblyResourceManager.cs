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
            return GetResourceStream(Assembly.GetExecutingAssembly(), resource);
        }

        public static string GetResourceString(string resource)
        {
            return GetResourceString(Assembly.GetExecutingAssembly(), resource, Encoding.UTF8);

        }

        public static string GetResourceString(Assembly assembly, string resource, Encoding encoding)
        {
            using (var stream = new StreamReader(GetResourceStream(assembly, resource), encoding))
            {
                return stream.ReadToEnd();
            }
        }

        public static Stream GetResourceStream(Assembly assembly, string resource)
        {
            return assembly.GetManifestResourceStream(resource);
        }
    }
}
