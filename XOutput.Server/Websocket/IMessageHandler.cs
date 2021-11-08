using System;
using Microsoft.AspNetCore.Http;
using NLog;
using XOutput.Threading;
using XOutput.Websocket.Common;

namespace XOutput.Websocket
{
    public interface IMessageHandler
    {
        void Handle(MessageBase message);
        void Close();
    }

    public abstract class MessageHandler : IMessageHandler {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly ThreadContext pingThreadContext;
        protected readonly CloseFunction closeFunction;
        protected readonly SenderFunction senderFunction;

        protected MessageHandler(CloseFunction closeFunction, SenderFunction senderFunction) {
            this.closeFunction = closeFunction;
            this.senderFunction = senderFunction;
            pingThreadContext = ThreadCreator.CreateLoop("Ping loop", () => {
                try
                {
                    var r = new PingRequest {
                        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    };
                    senderFunction(new PingRequest {
                        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    });
                }
                catch (Exception ex)
                {
                    logger.Warn(ex, "Ping failed, closing connection");
                    closeFunction();
                }
            }, 5000).Start();
        }

        public void Handle(MessageBase message) {
            if (message is PingRequest) {
                senderFunction(new PongResponse { Timestamp = (message as PingRequest).Timestamp });
            } else if (message is PongResponse) {
                logger.Debug(() => $"Delay is {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - (message as PongResponse).Timestamp} ms");
            } else if (message is DebugRequest) {
                var debugMessage = message as DebugRequest;
                logger.Info("Message from client: " + debugMessage.Data);
            } else {
                HandleMessage(message);
            }
        }

        protected virtual void HandleMessage(MessageBase message) {
            throw new NotImplementedException();
        }

        public virtual void Close() {
            pingThreadContext.Cancel();
        }
    }
}
