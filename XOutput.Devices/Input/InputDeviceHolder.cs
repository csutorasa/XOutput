using System;
using System.Collections.Generic;

namespace XOutput.Devices.Input
{
    public class InputDeviceHolder : IDisposable
    {
        event DeviceInputChangedHandler InputChanged;
        IEnumerable<InputSource> Sources { get; }
        IEnumerable<ForceFeedbackTarget> ForceFeedbacks { get; }
        string DisplayName { get; }
        string InterfacePath { get; }
        string UniqueId { get; }
        string HardwareID { get; }

        protected bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            if (disposing)
            {
                
            }
            disposed = true;
        }

        public InputSource FindSource(int offset)
        {
            return null;
        }
        public ForceFeedbackTarget FindTarget(int offset)
        {
            return null;
        }
        public void SetForceFeedback(int offset, double value)
        {

        }
    }
}
