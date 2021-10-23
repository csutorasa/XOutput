namespace XOutput.Websocket
{
    public interface IMessageHandler
    {
        bool CanHandle(MessageBase message);
        void Handle(MessageBase message);
        void Close();
    }
}
