using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Diagnostics;

namespace XOutput.Devices.Input
{
    public class InputDiagnostics : IDiagnostics
    {
        protected IInputDevice device;

        public object Source => device;

        public InputDiagnostics(IInputDevice device)
        {
            this.device = device;
        }

        public IEnumerable<DiagnosticsResult> GetResults()
        {
            return new DiagnosticsResult[]
            {
                GetAxesResult(),
                GetButtonsResult(),
                GetDPadResult(),
                GetForceFeedbackResult(),
            };
        }

        public DiagnosticsResult GetAxesResult()
        {
            int axesCount = device.Axes.Count();
            DiagnosticsResult result = new DiagnosticsResult
            {
                Value = axesCount,
                Type = InputDiagnosticsTypes.AxesCount,
            };
            if (axesCount < 4)
            {
                result.State = DiagnosticsResultState.Warning;
            }
            else
            {
                result.State = DiagnosticsResultState.Passed;
            }
            return result;
        }

        public DiagnosticsResult GetButtonsResult()
        {
            int buttonsCount = device.Buttons.Count();
            DiagnosticsResult result = new DiagnosticsResult
            {
                Value = buttonsCount,
                Type = InputDiagnosticsTypes.ButtonsCount,
            };
            if (buttonsCount < 8)
            {
                result.State = DiagnosticsResultState.Warning;
            }
            else
            {
                result.State = DiagnosticsResultState.Passed;
            }
            return result;
        }

        public DiagnosticsResult GetDPadResult()
        {
            int dPadsCount = device.DPads.Count();
            DiagnosticsResult result = new DiagnosticsResult
            {
                Value = dPadsCount,
                Type = InputDiagnosticsTypes.DPadCount,
            };
            if (dPadsCount < 1)
            {
                result.State = DiagnosticsResultState.Warning;
            }
            else
            {
                result.State = DiagnosticsResultState.Passed;
            }
            return result;
        }

        public DiagnosticsResult GetForceFeedbackResult()
        {
            int forceFeedbackCount = device.ForceFeedbackCount;
            DiagnosticsResult result = new DiagnosticsResult
            {
                Value = forceFeedbackCount,
                Type = InputDiagnosticsTypes.ForceFeedbackCount,
            };
            if (forceFeedbackCount < 1)
            {
                result.State = DiagnosticsResultState.Warning;
            }
            else
            {
                result.State = DiagnosticsResultState.Passed;
            }
            return result;
        }
    }
}
