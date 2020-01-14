using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using XOutput.Api.Message;

namespace XOutput.Server.Websocket
{
    interface IMessageHandler
    {
        Type HandledType { get; }

        void HandleMessage(MessageBase message, WebsocketSessionContext context);
    }

    abstract class MessageHandler<T> : IMessageHandler where T : MessageBase
    {
        public Type HandledType => typeof(T);

        public void HandleMessage(MessageBase message, WebsocketSessionContext context)
        {
            Handle(message as T, context);
        }

        public abstract void Handle(T message, WebsocketSessionContext context);
    }
}
