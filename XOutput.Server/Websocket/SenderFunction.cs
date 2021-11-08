using System.Threading.Tasks;

namespace XOutput.Websocket
{
    public delegate Task SenderFunction(MessageBase message);

    public delegate Task SenderFunction<in T>(T message) where T : MessageBase;
}
