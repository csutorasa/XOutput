using NLog;
using System;
using System.Timers;
using XOutput.Api.Message;

namespace XOutput.Server.Websocket
{
    class PingMessageHandler : IMessageHandler
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly SenderFunction<PingMessage> senderFunction;
        private readonly CloseFunction closeFunction;
        private readonly Timer timer;

        public PingMessageHandler(CloseFunction closeFunction, SenderFunction<PingMessage> senderFunction)
        {
            this.closeFunction = closeFunction;
            this.senderFunction = senderFunction;
            timer = new Timer(5000);
            timer.Elapsed += TimerElapsed;
            timer.Start();
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                senderFunction(new PingMessage());
            }
            catch (Exception ex)
            {
                logger.Warn(ex, "Ping failed, closing connection");
                closeFunction();
            }
        }

        public bool CanHandle(MessageBase message)
        {
            return message is PingMessage;
        }

        public void Handle(MessageBase message)
        {

        }

        public void Close()
        {
            timer.Stop();
            timer.Elapsed -= TimerElapsed;
        }
    }
}
