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
        public DirectDevice DirectInput { get { return directInput; } }
        public XDevice XInput { get { return xInput; } }
        public DirectToXInputMapper Mapper { get { return mapper; } }
        public string DisplayName { get { return directInput.DisplayName; } }
        public int ControllerCount { get { return controllerCount; } }

        private static readonly Controllers controllers = new Controllers();
        protected readonly DirectDevice directInput;
        protected readonly DirectToXInputMapper mapper;
        protected readonly XDevice xInput;
        protected readonly ScpDevice scpDevice;
        protected Thread thread;
        protected bool running;
        private int controllerCount = 0;
        public GameController(DirectDevice directInput, DirectToXInputMapper mapper)
        {
            this.directInput = directInput;
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
            directInput.Dispose();
            scpDevice.Dispose();
        }
        public override string ToString()
        {
            return directInput.ToString();
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
                        while (running)
                        {
                            directInput.RefreshInput();
                            if (!scpDevice.Report(controllerCount, XInput.GetBinary())) break;
                            Thread.Sleep(1);
                        }
                    }
                    finally
                    {
                        scpDevice.Unplug(controllerCount);
                        onStop?.Invoke();
                    }
                });
                running = true;
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
            if (thread != null)
                thread.Abort();
            scpDevice.Unplug(controllerCount);
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
