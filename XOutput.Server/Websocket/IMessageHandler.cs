using XOutput.Api.Message;

namespace XOutput.Server.Websocket
{
    public interface IMessageHandler
    {
        bool CanHandle(MessageBase message);
        void Handle(MessageBase message);
        void Close();
    }
}
