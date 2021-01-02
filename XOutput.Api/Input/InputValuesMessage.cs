using System;
using System.Collections.Generic;

namespace XOutput.Api.Message.Input
{
    public class InputValuesMessage : MessageBase
    {
        public const string MessageType = "InputValues";

        public InputValuesMessage()
        {
            Type = MessageType;
        }

        public List<InputValueData> Values { get; set; }
    }

    public class InputValueData {
        public int Offset { get; set; }
        public string Method { get; set; }
        public double Value { get; set; }

        public override bool Equals(object obj)
        {
            return obj is InputValueData data &&
                   Offset == data.Offset &&
                   Method == data.Method;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Offset, Method);
        }
    }
}
