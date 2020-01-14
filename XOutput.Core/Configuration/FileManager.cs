using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Core.Configuration
{
    public class FileManager
    {
        public string ReadFile(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        public void WriteFile(string filePath, string text)
        {
            File.WriteAllText(filePath, text);
        }
    }
}
