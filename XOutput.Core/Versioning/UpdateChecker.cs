using System;
using System.Threading.Tasks;

namespace XOutput.Versioning
{
    public sealed class UpdateChecker
    {
        private readonly IVersionGetter versionGetter;

        public UpdateChecker(IVersionGetter versionGetter)
        {
            this.versionGetter = versionGetter;
        }

        /// <summary>
        /// Compares the current version with the latest release.
        /// </summary>
        /// <returns></returns>
        public async Task<VersionCompare> CompareReleaseAsync(string appVersion)
        {
            VersionCompare compare;
            try
            {
                string latestRelease = await versionGetter.GetLatestReleaseAsync();
                compare = Version.Compare(appVersion, latestRelease);
            }
            catch (Exception)
            {
                compare = new VersionCompare {
                    Result = VersionCompareValues.Error,
                    CurrentVersion = appVersion,
                    LatestVersion = null,
                };
            }
            return await Task.Run(() => compare);
        }
    }
}
