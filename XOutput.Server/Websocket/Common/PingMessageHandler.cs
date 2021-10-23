using NLog;
using System;
using System.Timers;

namespace XOutput.Websocket.Common
{
    class PingMessageHandler : IMessageHandler
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly SenderFunction<PingRequest> pingSenderFunction;
        private readonly SenderFunction<PongResponse> pongSenderFunction;
        private readonly CloseFunction closeFunction;
        private readonly Timer timer;

        public PingMessageHandler(SenderFunction<PingRequest> pingSenderFunction, SenderFunction<PongResponse> pongSenderFunction, CloseFunction closeFunction)
        {
            this.pingSenderFunction = pingSenderFunction;
            this.pongSenderFunction = pongSenderFunction;
            this.closeFunction = closeFunction;
            timer = new Timer(5000);
            timer.Elapsed += TimerElapsed;
            timer.Start();
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                pingSenderFunction(new PingRequest {
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                });
            }
            catch (Exception ex)
            {
                logger.Warn(ex, "Ping failed, closing connection");
                closeFunction();
            }
        }

        public bool CanHandle(MessageBase message)
        {
            return message is PingRequest || message is PongResponse;
        }

        public void Handle(MessageBase message)
        {
            if (message is PingRequest) {
                pongSenderFunction(new PongResponse { Timestamp = (message as PingRequest).Timestamp });
            } else if (message is PongResponse) {
                logger.Debug(() => $"Delay is {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - (message as PongResponse).Timestamp}");
            }
        }

        public void Close()
        {
            timer.Stop();
            timer.Elapsed -= TimerElapsed;
        }
    }
}
