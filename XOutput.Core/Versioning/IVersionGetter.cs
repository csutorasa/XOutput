using NLog;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using XOutput.DependencyInjection;

namespace XOutput.Versioning
{
    public interface IVersionGetter
    {
        Task<string> GetLatestReleaseAsync();
    }
}
