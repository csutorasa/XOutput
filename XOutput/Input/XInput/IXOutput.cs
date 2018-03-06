using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Input.XInput
{
    public interface IXOutput : IDisposable
    {
        bool Plugin(int controllerCount);
        bool Unplug(int controllerCount);
        bool Report(int controllerCount, Dictionary<XInputTypes, double> values);
    }
}
