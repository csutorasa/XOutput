using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XOutput.Input.DirectInput;
using XOutput.Input.Mapper;
using XOutput.Input.XInput;

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
        protected readonly ScpDevice scpDevice;
        protected Thread thread;
        protected bool running;
        private int controllerCount = 0;
        public GameController(IInputDevice directInput, InputMapperBase mapper)
        {
            this.inputDevice = directInput;
            this.mapper = mapper;
            scpDevice = new ScpDevice();
            xInput = new XDevice(directInput, mapper);
            running = false;
        }
        ~GameController()
        {
            Dispose();
        }
        public void Dispose()
        {
            Stop();
            inputDevice.Dispose();
            scpDevice?.Dispose();
        }
        public override string ToString()
        {
            return inputDevice.ToString();
        }

        public int Start(Action onStop = null)
        {
            controllerCount = controllers.GetId();
            if (scpDevice.Unplug(controllerCount))
            {
                Thread.Sleep(10);
            }
            if (scpDevice.Plugin(controllerCount))
            {
                thread = new Thread(() =>
                {
                    try
                    {
                        XInput.InputChanged += () =>
                        {
                            if (!scpDevice.Report(controllerCount, XInput.GetBinary()))
                                running = false;
                        };
                        while (running)
                        {
                            Thread.Sleep(100);
                        }
                    }
                    finally
                    {
                        scpDevice.Unplug(controllerCount);
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
            scpDevice?.Unplug(controllerCount);
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
