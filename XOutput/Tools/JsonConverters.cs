using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Devices;
using XOutput.Devices.Input.Settings;

namespace XOutput.Tools
{
    public class InputTypeConverter : JsonConverter
    {
        public override bool CanRead => true;
        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return typeof(IDictionary<InputType, InputSettings>).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            IDictionary<InputType, InputSettings> dict = (IDictionary<InputType, InputSettings>)existingValue ?? new Dictionary<InputType, InputSettings>();
            foreach (var prop in obj.Properties())
            {
                dict.Add(InputType.Parse(prop.Name), prop.Value.ToObject<InputSettings>());
            }
            return dict;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
