using System.Threading.Tasks;
using XOutput.Api.Message;

namespace XOutput.Server.Websocket
{
    public delegate Task SenderFunction(MessageBase message);

    public delegate Task SenderFunction<in T>(T message) where T : MessageBase;

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
