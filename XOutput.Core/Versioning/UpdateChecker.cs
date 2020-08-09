using NLog;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using XOutput.Core.DependencyInjection;

namespace XOutput.Core.Versioning
{
    public sealed class UpdateChecker
    {
        private readonly IVersionGetter versionGetter;

        [ResolverMethod]
        public UpdateChecker(IVersionGetter versionGetter)
        {
            this.versionGetter = versionGetter;
        }

        /// <summary>
        /// Compares the current version with the latest release.
        /// </summary>
        /// <returns></returns>
        public async Task<VersionCompare> CompareRelease()
        {
            VersionCompare compare;
            try
            {
                string latestRelease = await versionGetter.GetLatestRelease();
                compare = Version.Compare(latestRelease);
            }
            catch (Exception)
            {
                compare = new VersionCompare {
                    Result = VersionCompareValues.Error,
                    CurrentVersion = Version.AppVersion,
                    LatestVersion = null,
                };
            }
            return await Task.Run(() => compare);
        }
    }
}
