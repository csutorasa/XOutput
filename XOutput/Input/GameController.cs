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

namespace XOutput.Input
{
    /// <summary>
    /// GameController is a container for input devices, mappers and output devices.
    /// </summary>
    public class GameController : IDisposable
    {
        /// <summary>
        /// Gets the input device
        /// </summary>
        public IInputDevice InputDevice => inputDevice;
        /// <summary>
        /// Gets the output device
        /// </summary>
        public XDevice XInput => xInput;
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

        private static readonly Controllers controllers = new Controllers();

        protected readonly IInputDevice inputDevice;
        protected readonly InputMapperBase mapper;
        protected readonly XDevice xInput;
        protected readonly IXOutput xOutput;
        protected Thread thread;
        protected bool running;
        private int controllerCount = 0;

        public GameController(IInputDevice directInput, InputMapperBase mapper)
        {
            inputDevice = directInput;
            this.mapper = mapper;
            xOutput = createXOutput();
            xInput = new XDevice(directInput, mapper);
            running = false;
        }

        private IXOutput createXOutput()
        {
            if (VigemDevice.IsAvailable())
            {
                return new VigemDevice();
            }
            else if (ScpDevice.IsAvailable())
            {
                return new ScpDevice();
            }
            else
            {
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
            xOutput.Dispose();
        }

        /// <summary>
        /// Starts the emulation of the device
        /// </summary>
        public int Start(Action onStop = null)
        {
            controllerCount = controllers.GetId();
            if (xOutput.Unplug(controllerCount))
            {
                // Wait for unplugging
                Thread.Sleep(10);
            }
            if (xOutput.Plugin(controllerCount))
            {
                thread = new Thread(() => ReadAndReportValues(onStop));
                running = true;
                thread.Name = $"Emulated controller {controllerCount} output refresher";
                thread.IsBackground = true;
                thread.Start();
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
            xOutput?.Unplug(controllerCount);
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
                xOutput.Unplug(controllerCount);
                onStop?.Invoke();
            }
        }

        private void XInput_InputChanged()
        {
            if (!xOutput.Report(controllerCount, XInput.GetValues()) || !inputDevice.Connected)
                running = false;
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
