using NLog;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using XOutput.Core.DependencyInjection;

namespace XOutput.Core.Versioning
{
    public interface IVersionGetter
    {
        Task<string> GetLatestRelease();
    }
}
