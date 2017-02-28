using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput
{
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

        public Dictionary<string, Dictionary<string, string>> Content { get { return content; } }

        public IniData()
        {

        }

        public void AddSection(string sectionName)
        {
            content.Add(sectionName, new Dictionary<string, string>());
        }

        public void AddSection(string sectionName, Dictionary<string, string> data)
        {
            content.Add(sectionName, data);
        }

        public Dictionary<string, string> GetSection(string sectionName)
        {
            return content[sectionName];
        }

        public string Serialize()
        {
            return string.Join(Environment.NewLine, content.Select(section => string.Join(Environment.NewLine, new string[] { $"[{section.Key}]" }.Concat(section.Value.Select(valuePair => $"{escape(valuePair.Key)}={escape(valuePair.Value)}")).ToArray())));
        }

        public void Serialize(StreamWriter sw)
        {
            foreach(var section in content)
            {
                sw.Write("[");
                sw.Write(section.Key);
                sw.WriteLine("]");
                foreach(var valuePair in section.Value)
                {
                    sw.Write(valuePair.Key);
                    sw.Write("=");
                    sw.WriteLine(valuePair.Value);
                }
            }
        }

        private string escape(string text)
        {
            var newText = text;
            foreach (var espacePair in escapes)
            {
                newText = newText.Replace(espacePair.Key, espacePair.Value);
            }
            return newText;
        }

        public static IniData Deserialize(string text)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            {
                using (var reader = new StreamReader(ms))
                {
                    return Deserialize(reader);
                }
            }
        }

        public static IniData Deserialize(StreamReader sr)
        {
            var ini = new IniData();

            // 0 readsectionheader
            // 1 readsectiondata
            int state = 0;
            string line;
            string sectionHeader = null;
            Dictionary<string, string> data = new Dictionary<string, string>();
            while((line = sr.ReadLine()) != null)
            {
                line = removeComment(line);
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                switch (state)
                {
                    case 0:
                        sectionHeader = readSectionHeader(line);
                        state = 1;
                        break;
                    case 1:
                        if(line[0] == '[')
                        {
                            ini.AddSection(sectionHeader, data);
                            data = new Dictionary<string, string>();
                            sectionHeader = readSectionHeader(line);
                        }
                        else
                        {
                            var valuePair = readValue(line);
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

        private static string removeComment(string line)
        {
            int indexOfComment = 0;
            while (indexOfComment >= 0)
            {
                indexOfComment = Math.Max(line.IndexOf('#', indexOfComment + 1), line.IndexOf(';', indexOfComment + 1));
                if(indexOfComment != -1 && (indexOfComment == 0 || line[indexOfComment-1] != '\\'))
                {
                    return line.Substring(0, indexOfComment);
                }
            }
            return line;
        }

        private static string readSectionHeader(string line)
        {
            if (line[0] != '[' || line[line.Length - 1] != ']')
                throw new ArgumentException($"Invalid section definition: {line}!");
            string section = unescape(line.Substring(1, line.Length - 2));
            if (string.IsNullOrWhiteSpace(section))
                throw new ArgumentException($"Empty section definition found!");
            return section;
        }

        private static KeyValuePair<string, string> readValue(string line)
        {
            int equalsValue = line.IndexOf('=');
            if(equalsValue < 0)
                throw new ArgumentException($"Invalid data line conatins no '=': {line}!");
            if (equalsValue == 0)
                throw new ArgumentException($"Invalid data line conatins no key: {line}!");
            string key = line.Substring(0, equalsValue);
            string value = line.Substring(equalsValue + 1);
            return new KeyValuePair<string, string>(key, value);
        }

        private static string unescape(string text)
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
