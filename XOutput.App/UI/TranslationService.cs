using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using XOutput.DependencyInjection;

namespace XOutput.App.UI
{
    public sealed class TranslationService
    {
        public string DefaultLanguage => defaultLanguage;

        private readonly Dictionary<string, string> data = new Dictionary<string, string>();
        private readonly string defaultLanguage;

        [ResolverMethod]
        public TranslationService()
        {
            string cultureName = CultureInfo.CurrentCulture.Name;
            if (cultureName.StartsWith("hu-"))
            {
                defaultLanguage = "Hungarian";
            }
            else 
            {
                defaultLanguage = "English";
            }
        }

        public string[] GetAvailableLanguages()
        {
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceNames()
                .Where(s => s.StartsWith(assembly.GetName().Name + ".Resources.Translations.", StringComparison.CurrentCultureIgnoreCase))
                .Select(resourceName => resourceName.Split('.')[4]).ToArray();
        }

        public bool Load(string language)
        {
            var assembly = Assembly.GetExecutingAssembly();
            foreach (var resourceName in assembly.GetManifestResourceNames().Where(s => s.StartsWith(assembly.GetName().Name + ".Resources.Translations.", StringComparison.CurrentCultureIgnoreCase)))
            {
                string resourceKey = resourceName.Split('.')[4];
                if (resourceKey == language)
                {
                    using (var stream = new StreamReader(assembly.GetManifestResourceStream(resourceName)))
                    {
                        var translation = JsonSerializer.Deserialize<JsonElement>(stream.ReadToEnd());
                        data.Clear();
                        Traverse("", translation);
                    }
                    TranslationModel.Instance.Language = language;
                    return true;
                }
            }
            return false;
        }

        private void Traverse(string prefix, JsonElement obj)
        {
            if (obj.ValueKind == JsonValueKind.String)
            {
                data[prefix] = obj.GetString();
            }
            else if (obj.ValueKind == JsonValueKind.Object)
            {
                var enumerator = obj.EnumerateObject();
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    string newPrefix = prefix == "" ? current.Name : $"{prefix}.{current.Name}";
                    Traverse(newPrefix, current.Value);
                }
            }
        }

        public string Translate(string key)
        {
            if (data.ContainsKey(key))
            {
                return data[key];
            }
            return key;
        }
    }
}
