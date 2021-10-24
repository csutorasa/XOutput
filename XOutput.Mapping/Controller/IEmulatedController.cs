using System.Collections.Generic;
using XOutput.Emulation;

namespace XOutput.Mapping.Controller
{
    public interface IEmulatedController {
        string Id { get; }
        string Name { get; }
        IDevice Device { get; }

        Dictionary<string, double> GetSources();
        Dictionary<string, double> GetTargets();
    }
}
