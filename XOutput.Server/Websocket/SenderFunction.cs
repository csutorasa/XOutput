using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Api.Message;

namespace XOutput.Server.Websocket
{
    public delegate Task SenderFunction(MessageBase message);

    public delegate Task SenderFunction<T>(T message) where T : MessageBase;

    public static class SenderFunctionHelper
    {
        public static SenderFunction<T> GetTyped<T>(this SenderFunction method) where T : MessageBase
        {
            return (T message) =>
            {
                return method(message);
            };
        }
    }
}
