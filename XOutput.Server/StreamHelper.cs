using Newtonsoft.Json;
using System.IO;

namespace XOutput.Server
{
    public static class StreamHelper
    {
        public static void WriteText(this Stream outputStream, string text)
        {
            using (StreamWriter sw = new StreamWriter(outputStream))
            {
                sw.Write(text);
            }
        }

        public static void WriteJson<T>(this Stream outputStream, T obj)
        {
            using (StreamWriter sw = new StreamWriter(outputStream))
            {
                string text = JsonConvert.SerializeObject(obj);
                sw.Write(text);
            }
        }
    }
}
