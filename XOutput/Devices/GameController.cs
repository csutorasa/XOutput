using System;
using System.Linq;
using System.Threading;
using XOutput.Devices.Input;
using XOutput.Devices.Input.DirectInput;
using XOutput.Devices.Mapper;
using XOutput.Devices.XInput;
using XOutput.Devices.XInput.SCPToolkit;
using XOutput.Devices.XInput.Vigem;
using XOutput.Logging;
using XOutput.Tools;
using XOutput.Core.DependencyInjection;
using XOutput.Core.Threading;

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
        /// Gets the number of the controller.
        /// </summary>
        public int ControllerCount => controllerCount;
        /// <summary>
        /// Gets if any XInput emulation is installed.
        /// </summary>
        public bool HasXOutputInstalled => xOutputManager.HasDevice;
        /// <summary>
        /// Gets if force feedback is supported.
        /// </summary>
        public bool ForceFeedbackSupported => xOutputManager.IsVigem;
        /// <summary>
        /// Gets the force feedback device.
        /// </summary>
        public IInputDevice ForceFeedbackDevice { get; set; }

        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GameController));

        private readonly InputMapper mapper;
        private readonly XOutputDevice xInput;
        private readonly XOutputManager xOutputManager;
        private ThreadContext threadContext;
        private bool running;
        private int controllerCount = 0;
        private Nefarius.ViGEm.Client.Targets.IXbox360Controller controller;

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
            if (!HasXOutputInstalled)
            {
                return 0;
            }
            if (controller != null)
            {
                controller.FeedbackReceived -= ControllerFeedbackReceived;
            }
            if (xOutputManager.Stop(controllerCount))
            {
                // Wait for unplugging
                Thread.Sleep(10);
            }
            controllerCount = xOutputManager.Start();
            if (controllerCount != 0)
            {
                threadContext = ThreadCreator.Create($"Emulated controller {controllerCount} output refresher", token => ReadAndReportValues(token, onStop)).Start();
                running = true;
                logger.Info($"Emulation started on {ToString()}.");
                if (ForceFeedbackSupported)
                {
                    logger.Info($"Force feedback mapping is connected on {ToString()}.");
                    controller = ((VigemDevice)xOutputManager.XOutputDevice).GetController(controllerCount);
                    controller.FeedbackReceived += ControllerFeedbackReceived;
                }
            }
            return controllerCount;
        }

        /// <summary>
        /// Stops the emulation of the device
        /// </summary>
        public void Stop()
        {
            if (running)
            {
                running = false;
                XInput.InputChanged -= XInputInputChanged;
                if (ForceFeedbackSupported)
                {
                    controller.FeedbackReceived -= ControllerFeedbackReceived;
                    logger.Info($"Force feedback mapping is disconnected on {ToString()}.");
                }
                xOutputManager.Stop(controllerCount);
                controllerCount = 0;
                logger.Info($"Emulation stopped on {ToString()}.");
                if (threadContext != null) {
                    threadContext.Cancel().Wait();
                }
            }
        }

        public override string ToString()
        {
            return DisplayName;
        }

        private void ReadAndReportValues(CancellationToken token, Action onStop)
        {
            XInput.InputChanged += XInputInputChanged;
            try
            {
                while (running && !token.IsCancellationRequested)
                {
                    Thread.Sleep(100);
                }
            } catch (Exception)
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
            if (!xOutputManager.XOutputDevice.Report(controllerCount, XInput.GetValues()))
            {
                Stop();
            }
        }

        private void ControllerFeedbackReceived(object sender, Nefarius.ViGEm.Client.Targets.Xbox360.Xbox360FeedbackReceivedEventArgs e)
        {
            ForceFeedbackDevice?.SetForceFeedback((double)e.LargeMotor / byte.MaxValue, (double)e.SmallMotor / byte.MaxValue);
        }
    }
}
