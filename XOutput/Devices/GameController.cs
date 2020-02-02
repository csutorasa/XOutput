using NLog;
using System;
using System.Linq;
using System.Threading;
using XOutput.Api.Message.Xbox;
using XOutput.Core.DependencyInjection;
using XOutput.Core.Threading;
using XOutput.Devices.Input;
using XOutput.Devices.Input.DirectInput;
using XOutput.Devices.Mapper;
using XOutput.Devices.XInput;

namespace XOutput.Devices
{
    /// <summary>
    /// GameController is a container for input devices, mappers and output devices.
    /// </summary>
    public sealed class GameController : IDisposable
    {
        /// <summary>
        /// Gets the output device.
        /// </summary>
        public XOutputDevice XInput => xInput;
        /// <summary>
        /// Gets the mapping of the input device.
        /// </summary>
        public InputMapper Mapper => mapper;
        /// <summary>
        /// Gets the name of the input device.
        /// </summary>
        public string DisplayName => mapper.Name;
        /// <summary>
        /// Gets the force feedback device.
        /// </summary>
        public IInputDevice ForceFeedbackDevice { get; set; }

        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly InputMapper mapper;
        private readonly XOutputDevice xInput;
        private readonly XOutputManager xOutputManager;
        private ThreadContext threadContext;
        private bool running;
        private WebsocketXboxClient client;

        public GameController(InputMapper mapper)
        {
            this.mapper = mapper;
            xOutputManager = ApplicationContext.Global.Resolve<XOutputManager>();
            xInput = new XOutputDevice(mapper);
            if (!string.IsNullOrEmpty(mapper.ForceFeedbackDevice))
            {
                var device = InputDevices.Instance.GetDevices().OfType<DirectDevice>().FirstOrDefault(d => d.UniqueId == mapper.ForceFeedbackDevice);
                if (device != null)
                {
                    ForceFeedbackDevice = device;
                }
            }
            running = false;
        }

        /// <summary>
        /// Disposes all used resources
        /// </summary>
        public void Dispose()
        {
            Stop();
            xInput?.Dispose();
        }

        /// <summary>
        /// Starts the emulation of the device
        /// </summary>
        public int Start(Action onStop = null)
        {
            Stop();
            client = xOutputManager.Start();
            threadContext = ThreadCreator.Create($"Emulated controller output refresher", token => ReadAndReportValues(onStop, token)).Start();
            running = true;
            logger.Info($"Emulation started on {ToString()}.");
            logger.Info($"Force feedback mapping is connected on {ToString()}.");
            client.Feedback += FeedbackReceived;
            // TODO
            return 1;
        }

        public void Stop()
        {
            if (running)
            {
                running = false;
                XInput.InputChanged -= XInputInputChanged;
                client.Feedback -= FeedbackReceived;
                logger.Info($"Force feedback mapping is disconnected on {ToString()}.");
                xOutputManager.Stop(client);
                logger.Info($"Emulation stopped on {ToString()}.");
                if (threadContext != null)
                {
                    threadContext.Cancel().Wait();
                }
            }
        }

        public override string ToString()
        {
            return DisplayName;
        }

        private void ReadAndReportValues(Action onStop, CancellationToken token)
        {
            XInput.InputChanged += XInputInputChanged;
            try
            {
                while (running && !token.IsCancellationRequested)
                {
                    Thread.Sleep(100);
                }
            }
            catch (Exception)
            {
                Stop();
            }
            finally
            {
                onStop?.Invoke();
            }
        }

        private void XInputInputChanged(object sender, DeviceInputChangedEventArgs e)
        {
            try
            {
                client.SendInput(GetMessage(e));
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to write output");
                Stop();
            }
        }

        private void FeedbackReceived(object sender, XboxFeedbackMessage e)
        {
            ForceFeedbackDevice?.SetForceFeedback((double)e.Large / byte.MaxValue, (double)e.Small / byte.MaxValue);
        }

        private XboxInputMessage GetMessage(DeviceInputChangedEventArgs e)
        {
            XboxInputMessage message = new XboxInputMessage();
            if(e.ChangedDPads.Any())
            {
                message.UP = XInput.GetBool(XInputTypes.UP);
                message.DOWN = XInput.GetBool(XInputTypes.DOWN);
                message.LEFT = XInput.GetBool(XInputTypes.LEFT);
                message.RIGHT = XInput.GetBool(XInputTypes.RIGHT);
            }
            foreach (var value in e.ChangedValues)
            {
                if (value.IsButton)
                {
                    XInputTypes type = (XInputTypes) Enum.Parse(typeof(XInputTypes), value.DisplayName);
                    switch (type)
                    {
                        case XInputTypes.A:
                            message.A = value.Value > 0.5;
                            break;
                        case XInputTypes.B:
                            message.B = value.Value > 0.5;
                            break;
                        case XInputTypes.X:
                            message.X = value.Value > 0.5;
                            break;
                        case XInputTypes.Y:
                            message.Y = value.Value > 0.5;
                            break;
                        case XInputTypes.L1:
                            message.L1 = value.Value > 0.5;
                            break;
                        case XInputTypes.R1:
                            message.R1 = value.Value > 0.5;
                            break;
                        case XInputTypes.L3:
                            message.L3 = value.Value > 0.5;
                            break;
                        case XInputTypes.R3:
                            message.R3 = value.Value > 0.5;
                            break;
                        case XInputTypes.Start:
                            message.Start = value.Value > 0.5;
                            break;
                        case XInputTypes.Back:
                            message.Back = value.Value > 0.5;
                            break;
                        case XInputTypes.Home:
                            message.Home = value.Value > 0.5;
                            break;
                    }
                } 
                else
                {
                    XInputTypes type = (XInputTypes)Enum.Parse(typeof(XInputTypes), value.DisplayName);
                    switch (type)
                    {
                        case XInputTypes.L2:
                            message.L2 = value.Value;
                            break;
                        case XInputTypes.R2:
                            message.R2 = value.Value;
                            break;
                        case XInputTypes.LX:
                            message.LX = value.Value;
                            break;
                        case XInputTypes.LY:
                            message.LY = value.Value;
                            break;
                        case XInputTypes.RX:
                            message.RX = value.Value;
                            break;
                        case XInputTypes.RY:
                            message.RY = value.Value;
                            break;
                    }
                }
            }
            return message;
        }
    }
}
