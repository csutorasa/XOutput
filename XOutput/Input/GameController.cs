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
    public class GameController : IDisposable
    {
        public IInputDevice InputDevice => inputDevice;
        public XDevice XInput => xInput;
        public InputMapperBase Mapper => mapper;
        public string DisplayName => inputDevice.DisplayName;
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
            this.inputDevice = directInput;
            this.mapper = mapper;
            xOutput = createXOutput();
            xInput = new XDevice(directInput, mapper);
            running = false;
        }

        private IXOutput createXOutput()
        {
            if(VigemDevice.IsAvailable())
            {
                return new VigemDevice();
            }
            else if(ScpDevice.IsAvailable())
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
        public void Dispose()
        {
            Stop();
            inputDevice.Dispose();
            xOutput?.Dispose();
        }
        public override string ToString()
        {
            return inputDevice.ToString();
        }

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
                thread = new Thread(() =>
                {
                    try
                    {
                        XInput.InputChanged += () =>
                        {
                            if (!xOutput.Report(controllerCount, XInput.GetValues()))
                                running = false;
                        };
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
                });
                running = true;
                thread.IsBackground = true;
                thread.Start();
            }
            else
            {
                resetId();
            }
            return controllerCount;
        }

        public void Stop()
        {
            running = false;
            thread?.Abort();
            xOutput?.Unplug(controllerCount);
            resetId();
        }

        private void resetId()
        {
            if(controllerCount != 0)
            {
                controllers.DisposeId(controllerCount);
                controllerCount = 0;
            }
        }
    }
}
