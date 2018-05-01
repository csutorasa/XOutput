using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Devices.XInput.SCPToolkit;
using XOutput.Devices.XInput.Vigem;
using XOutput.Diagnostics;

namespace XOutput.Devices.XInput
{
    public class XInputDiagnostics : IDiagnostics
    {
        public object Source => null;

        public IEnumerable<DiagnosticsResult> GetResults()
        {
            return new DiagnosticsResult[]
            {
                GetScpDeviceResult(),
                GetVigemDeviceResult(),
                GetXDeviceResult(),
            };
        }

        public DiagnosticsResult GetXDeviceResult()
        {
            DiagnosticsResult result = new DiagnosticsResult
            {
                Type = XInputDiagnosticsTypes.XDevice,
            };
            if (GetVigemDeviceResult().State != DiagnosticsResultState.Passed)
            {
                if (GetScpDeviceResult().State != DiagnosticsResultState.Passed)
                {
                    result.Value = false;
                    result.State = DiagnosticsResultState.Failed;
                }
                else
                {
                    result.Value = true;
                    result.State = DiagnosticsResultState.Warning;
                }
            }
            else
            {
                result.Value = true;
                result.State = DiagnosticsResultState.Passed;
            }
            return result;
        }

        public DiagnosticsResult GetScpDeviceResult()
        {
            DiagnosticsResult result = new DiagnosticsResult
            {
                Type = XInputDiagnosticsTypes.ScpDevice,
            };
            if (ScpDevice.IsAvailable())
            {
                result.Value = true;
                result.State = DiagnosticsResultState.Passed;
            }
            else
            {
                result.Value = false;
                result.State = DiagnosticsResultState.Warning;
            }
            return result;
        }

        public DiagnosticsResult GetVigemDeviceResult()
        {
            DiagnosticsResult result = new DiagnosticsResult
            {
                Type = XInputDiagnosticsTypes.VigemDevice,
            };
            if (VigemDevice.IsAvailable())
            {
                result.Value = true;
                result.State = DiagnosticsResultState.Passed;
            }
            else
            {
                result.Value = false;
                result.State = DiagnosticsResultState.Warning;
            }
            return result;
        }
    }
}
