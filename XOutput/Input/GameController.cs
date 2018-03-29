using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XOutput.Input.DirectInput;
using XOutput.Input.Mapper;
using XOutput.Input.XInput;
using XOutput.Input.XInput.SCPToolkit;
using XOutput.Input.XInput.Vigem;
using XOutput.Logging;

namespace XOutput.Input
{
    /// <summary>
    /// GameController is a container for input devices, mappers and output devices.
    /// </summary>
    public sealed class GameController : IDisposable
    {
        /// <summary>
        /// Gets the input device
        /// </summary>
        public IInputDevice InputDevice => inputDevice;
        /// <summary>
        /// Gets the output device
        /// </summary>
        public XOutputDevice XInput => xInput;
        /// <summary>
        /// Gets the mapping of the input device
        /// </summary>
        public InputMapperBase Mapper => mapper;
        /// <summary>
        /// Gets the name of the input device
        /// </summary>
        public string DisplayName => inputDevice.DisplayName;
        /// <summary>
        /// Gets the number of the controller
        /// </summary>
        public int ControllerCount => controllerCount;

        public bool ForceFeedbackSupported => xOutputInterface is VigemDevice;

        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GameController));
        private static readonly Controllers controllers = new Controllers();

        private readonly IInputDevice inputDevice;
        private readonly InputMapperBase mapper;
        private readonly XOutputDevice xInput;
        private readonly IXOutputInterface xOutputInterface;
        private Thread thread;
        private bool running;
        private int controllerCount = 0;
        private Nefarius.ViGEm.Client.Targets.Xbox360Controller controller;

        public GameController(IInputDevice directInput, InputMapperBase mapper)
        {
            inputDevice = directInput;
            this.mapper = mapper;
            xOutputInterface = createXOutput();
            xInput = new XOutputDevice(directInput, mapper);
            if (mapper.SelectedDPad == -1 && directInput.DPads.Any())
                mapper.SelectedDPad = 0;
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
            inputDevice.Dispose();
            xInput.Dispose();
            xOutputInterface.Dispose();
        }

        /// <summary>
        /// Starts the emulation of the device
        /// </summary>
        public int Start(Action onStop = null)
        {
            controllerCount = controllers.GetId();
            if (controller != null)
            {
                controller.FeedbackReceived -= Controller_FeedbackReceived;
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
                if (ForceFeedbackSupported)
                {
                    logger.Info("Force feedback mapping is connected.");
                    controller = ((VigemDevice)xOutputInterface).GetController(controllerCount);
                    controller.FeedbackReceived += Controller_FeedbackReceived;
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
            running = false;
            thread?.Abort();
            XInput.InputChanged -= XInput_InputChanged;
            xOutputInterface?.Unplug(controllerCount);
            resetId();
        }

        public override string ToString()
        {
            return inputDevice.ToString();
        }

        private void ReadAndReportValues(Action onStop)
        {
            try
            {
                XInput.InputChanged += XInput_InputChanged;
                while (running)
                {
                    Thread.Sleep(100);
                }
            }
            finally
            {
                xOutputInterface.Unplug(controllerCount);
                onStop?.Invoke();
            }
        }

        private void XInput_InputChanged()
        {
            if (!xOutputInterface.Report(controllerCount, XInput.GetValues()) || !inputDevice.Connected)
                running = false;
        }

        private void Controller_FeedbackReceived(object sender, Nefarius.ViGEm.Client.Targets.Xbox360.Xbox360FeedbackReceivedEventArgs e)
        {
            inputDevice.SetForceFeedback((double)e.LargeMotor / byte.MaxValue, (double)e.SmallMotor / byte.MaxValue);
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
