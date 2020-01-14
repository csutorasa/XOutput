using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Api.Message
{
    public abstract class MessageBase
    {
        public string Type { get; set; }
    }
}
