using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XOutput.Devices.Input;
using XOutput.Devices.XInput;
using XOutput.Devices.XInput.SCPToolkit;
using XOutput.Devices.XInput.Settings;
using XOutput.Devices.XInput.Vigem;
using XOutput.Logging;

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
        /// Gets or sets the name of the output device.
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// Gets the number of the controller.
        /// </summary>
        public int ControllerCount => controllerCount;
        /// <summary>
        /// Gets if any XInput emulation is installed.
        /// </summary>
        public bool HasXOutputInstalled => xOutputInterface != null;
        /// <summary>
        /// Gets if force feedback is supported.
        /// </summary>
        public bool ForceFeedbackSupported => xOutputInterface is VigemDevice;

        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GameController));
        private static readonly Controllers controllers = new Controllers();

        private readonly XOutputDevice xInput;
        private readonly IXOutputInterface xOutputInterface;
        private readonly IEnumerable<ForceFeedbackSettings> forceFeedbackDevices;
        private Thread thread;
        private bool running;
        private int controllerCount = 0;
        private Nefarius.ViGEm.Client.Targets.Xbox360Controller controller;

        public GameController(Dictionary<XInputTypes, MapperSettings> mappers, DPadSettings dpad, IEnumerable<ForceFeedbackSettings> forceFeedbackDevices)
        {
            xOutputInterface = createXOutput();
            xInput = new XOutputDevice(mappers, dpad);
            this.forceFeedbackDevices = forceFeedbackDevices;
            running = false;
        }

        private IXOutputInterface createXOutput()
        {
            if (VigemDevice.IsAvailable())
            {
                logger.Info("ViGEm devices are used.");
                return new VigemDevice();
            }
            else if (ScpDevice.IsAvailable())
            {
                logger.Info("SCP Toolkit devices are used.");
                return new ScpDevice();
            }
            else
            {
                logger.Warning("Neither ViGEm nor SCP devices can be used.");
                return null;
            }
        }

        ~GameController()
        {
            Dispose();
        }

        /// <summary>
        /// Disposes all used resources
        /// </summary>
        public void Dispose()
        {
            Stop();
            xInput.Dispose();
            xOutputInterface.Dispose();
        }

        /// <summary>
        /// Starts the emulation of the device
        /// </summary>
        public int Start(Action onStop = null)
        {
            if (!HasXOutputInstalled)
                return 0;
            controllerCount = controllers.GetId();
            if (controller != null)
            {
                controller.FeedbackReceived -= ControllerFeedbackReceived;
            }
            if (xOutputInterface.Unplug(controllerCount))
            {
                // Wait for unplugging
                Thread.Sleep(10);
            }
            if (xOutputInterface.Plugin(controllerCount))
            {
                thread = new Thread(() => ReadAndReportValues(onStop));
                running = true;
                thread.Name = $"Emulated controller {controllerCount} output refresher";
                thread.IsBackground = true;
                thread.Start();
                logger.Info($"Emulation started on {ToString()}.");
                if (ForceFeedbackSupported)
                {
                    logger.Info($"Force feedback mapping is connected on {ToString()}.");
                    controller = ((VigemDevice)xOutputInterface).GetController(controllerCount);
                    controller.FeedbackReceived += ControllerFeedbackReceived;
                }
            }
            else
            {
                resetId();
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
                xOutputInterface?.Unplug(controllerCount);
                logger.Info($"Emulation stopped on {ToString()}.");
                resetId();
                thread?.Abort();
            }
        }

        public MapperSettings GetMapping(XInputTypes type)
        {
            return XInput.GetMapping(type);
        }

        public DPadSettings GetDPadData()
        {
            return XInput.GetDPadData();
        }

        public override string ToString()
        {
            return DisplayName;
        }

        private void ReadAndReportValues(Action onStop)
        {
            try
            {
                XInput.InputChanged += XInputInputChanged;
                while (running)
                {
                    Thread.Sleep(100);
                }
            }
            finally
            {
                onStop?.Invoke();
                Stop();
            }
        }

        private void XInputInputChanged(object sender, DeviceInputChangedEventArgs e)
        {
            if (!xOutputInterface.Report(controllerCount, XInput.GetValues()))
                Stop();
        }

        private void ControllerFeedbackReceived(object sender, Nefarius.ViGEm.Client.Targets.Xbox360.Xbox360FeedbackReceivedEventArgs e)
        {
            foreach (var inputDevice in forceFeedbackDevices)
            {
                inputDevice.Device.SetForceFeedback((double)e.LargeMotor / byte.MaxValue, (double)e.SmallMotor / byte.MaxValue);
            }
        }

        private void resetId()
        {
            if (controllerCount != 0)
            {
                controllers.DisposeId(controllerCount);
                controllerCount = 0;
            }
        }
    }
}
