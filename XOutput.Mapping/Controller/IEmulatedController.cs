using System.Collections.Generic;

namespace XOutput.Mapping.Controller
{
    public interface IEmulatedController {
        string Id { get; }
        string Name { get; }

        Dictionary<string, double> GetSources();
        Dictionary<string, double> GetTargets();
    }
}
