using System.Collections.Generic;
using System.Linq;
using XOutput.Diagnostics;

namespace XOutput.Devices.Input
{
    /// <summary>
    /// Tests an input device.
    /// </summary>
    public class InputDiagnostics : IDiagnostics
    {
        protected IInputDevice device;

        /// <summary>
        /// Gets the source <see cref="IInputDevice"/>.
        /// <para>Implements <see cref="IDiagnostics.Source"/></para>
        /// </summary>
        public object Source => device;

        /// <summary>
        /// C
        /// </summary>
        /// <param name="device"></param>
        public InputDiagnostics(IInputDevice device)
        {
            this.device = device;
        }

        /// <summary>
        /// <para>Implements <see cref="IDiagnostics.GetResults()"/></para>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DiagnosticsResult> GetResults()
        {
            return new DiagnosticsResult[]
            {
                GetAxesResult(),
                GetButtonsResult(),
                GetDPadResult(),
                GetForceFeedbackResult(),
                GetSlidersResult(),
            };
        }

        public DiagnosticsResult GetAxesResult()
        {
            int axesCount = device.Sources.Count(s => InputSourceTypes.Axis.HasFlag(s.Type));
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

        public DiagnosticsResult GetSlidersResult()
        {
            int slidersCount = device.Sources.Count(s => s.Type == InputSourceTypes.Slider);
            return new DiagnosticsResult
            {
                Value = slidersCount,
                Type = InputDiagnosticsTypes.SlidersCount,
                State = DiagnosticsResultState.Passed,
            };
        }

        public DiagnosticsResult GetButtonsResult()
        {
            int buttonsCount = device.Sources.Count(s => s.Type == InputSourceTypes.Button);
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
