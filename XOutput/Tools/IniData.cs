using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Tools
{
    /// <summary>
    /// Reads ini-line data.
    /// </summary>
    public class IniData
    {
        private static readonly Dictionary<string, string> escapes = new Dictionary<string, string>(){
            { "\\", "\\\\"},
            { "\0", "\\0"},
            { "\t", "\\t"},
            { "\r", "\\r"},
            { "\n", "\\n"},
            { ";", "\\;"},
            { "#", "\\#"},
            { "=", "\\=;"},
            { ":", "\\:"}
        };

        private readonly Dictionary<string, Dictionary<string, string>> content = new Dictionary<string, Dictionary<string, string>>();
        /// <summary>
        /// Gets the content of the file.
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> Content => content;

        public IniData()
        {

        }

        /// <summary>
        /// Adds a new section. <c>[Section]</c>
        /// </summary>
        /// <param name="sectionName">name of the section</param>
        public void AddSection(string sectionName)
        {
            content.Add(sectionName, new Dictionary<string, string>());
        }

        /// <summary>
        /// Adds a new section with predefined data. <c>[Section]</c>
        /// </summary>
        /// <param name="sectionName">name of the section</param>
        /// <param name="data">predefined data</param>
        public void AddSection(string sectionName, Dictionary<string, string> data)
        {
            content.Add(sectionName, data);
        }

        /// <summary>
        /// Gets the content of a section.
        /// </summary>
        /// <param name="sectionName">name of the section</param>
        /// <returns></returns>
        public Dictionary<string, string> GetSection(string sectionName)
        {
            return content[sectionName];
        }

        /// <summary>
        /// Gets the content in string format.
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            return string.Join(Environment.NewLine, content.Select(section => string.Join(Environment.NewLine, new string[] { $"[{section.Key}]" }.Concat(section.Value.Select(valuePair => $"{EscapeText(valuePair.Key)}={EscapeText(valuePair.Value)}")).ToArray())));
        }

        /// <summary>
        /// Writes the content into the steam.
        /// </summary>
        /// <param name="sw">stream to write</param>
        public void Serialize(StreamWriter sw)
        {
            foreach (var section in content)
            {
                sw.Write("[");
                sw.Write(section.Key);
                sw.WriteLine("]");
                foreach (var valuePair in section.Value)
                {
                    sw.Write(valuePair.Key);
                    sw.Write("=");
                    sw.WriteLine(valuePair.Value);
                }
            }
        }

        private string EscapeText(string text)
        {
            var newText = text;
            foreach (var espacePair in escapes)
            {
                newText = newText.Replace(espacePair.Key, espacePair.Value);
            }
            return newText;
        }

        /// <summary>
        /// Parses the text.
        /// </summary>
        /// <param name="text">ini-like content</param>
        /// <returns></returns>
        public static IniData Deserialize(string text)
        {
            using (var reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(text))))
            {
                return Deserialize(reader);
            }
        }

        /// <summary>
        /// Parses the text.
        /// </summary>
        /// <param name="sr">stream to read</param>
        /// <returns></returns>
        public static IniData Deserialize(StreamReader sr)
        {
            var ini = new IniData();

            // 0 readsectionheader
            // 1 readsectiondata
            int state = 0;
            string line;
            string sectionHeader = null;
            Dictionary<string, string> data = new Dictionary<string, string>();
            for (int linecount = 1; (line = sr.ReadLine()) != null; linecount++)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                switch (state)
                {
                    case 0:
                        sectionHeader = ReadSectionHeader(line);
                        state = 1;
                        break;
                    case 1:
                        if (line[0] == '[')
                        {
                            ini.AddSection(sectionHeader, data);
                            data = new Dictionary<string, string>();
                            sectionHeader = ReadSectionHeader(line);
                        }
                        else
                        {
                            var valuePair = ReadValue(line);
                            data.Add(valuePair.Key, valuePair.Value);
                        }
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
            ini.AddSection(sectionHeader, data);
            return ini;
        }

        private static string ReadSectionHeader(string line)
        {
            if (line[0] != '[' || line[line.Length - 1] != ']')
                throw new ArgumentException($"Invalid section definition: {line}!");
            string section = UnescapeText(line.Substring(1, line.Length - 2));
            if (string.IsNullOrWhiteSpace(section))
                throw new ArgumentException($"Empty section definition found!");
            return section;
        }

        private static KeyValuePair<string, string> ReadValue(string line)
        {
            int equalsValue = line.IndexOf('=');
            if (equalsValue < 0)
                throw new ArgumentException($"Invalid data line conatins no '=': {line}!");
            if (equalsValue == 0)
                throw new ArgumentException($"Invalid data line conatins no key: {line}!");
            string key = line.Substring(0, equalsValue);
            string value = line.Substring(equalsValue + 1);
            return new KeyValuePair<string, string>(key, value);
        }

        private static string UnescapeText(string text)
        {
            var newText = text;
            foreach (var espacePair in escapes)
            {
                newText = newText.Replace(espacePair.Value, espacePair.Key);
            }
            return newText;
        }
    }
}
