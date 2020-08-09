using NLog;
using System;
using System.Linq;

namespace XOutput.Core.Versioning
{
    /// <summary>
    /// Version related informations.
    /// </summary>
    public static class Version
    {
        /// <summary>
        /// Current application version.
        /// </summary>
        public const string AppVersion = "3.27";

        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Compares the version with the current version.
        /// </summary>
        /// <param name="version">reference version</param>
        /// <returns></returns>
        public static VersionCompare Compare(string version)
        {
            string current = Version.AppVersion;
            return new VersionCompare
            {
                Result = DoCompare(current, version),
                CurrentVersion = current,
                LatestVersion = version,
            };
        }

        private static VersionCompareValues DoCompare(string appVersion, string version)
        {
            try
            {
                logger.Debug("Current application version: " + appVersion);
                logger.Debug("Latest application version: " + version);
                var current = appVersion.Split('.').Select(t => int.Parse(t)).ToArray();
                var compare = version.Split('.').Select(t => int.Parse(t)).ToArray();
                for (int i = 0; i < 100; i++)
                {
                    bool currentNotPresent = i >= current.Length;
                    bool compareNotPresent = i >= compare.Length;
                    if (compareNotPresent)
                    {
                        if (currentNotPresent)
                        {
                            return VersionCompareValues.UpToDate;
                        }
                        else
                        {
                            return VersionCompareValues.NewRelease;
                        }
                    }
                    else
                    {
                        if (currentNotPresent)
                        {
                            return VersionCompareValues.NeedsUpgrade;
                        }
                        else
                        {
                            int currentValue = current[i];
                            int compareValue = compare[i];
                            if (currentValue > compareValue)
                            {
                                return VersionCompareValues.NewRelease;
                            }
                            if (currentValue < compareValue)
                            {
                                return VersionCompareValues.NeedsUpgrade;
                            }
                        }
                    }
                }
                return VersionCompareValues.Error;
            }
            catch (Exception)
            {
                return VersionCompareValues.Error;
            }
        }
    }

    public class VersionCompare
    {
        public VersionCompareValues Result { get; set; }
        public string CurrentVersion { get; set; }
        public string LatestVersion { get; set; }
    }

    /// <summary>
    /// Version compare result enum.
    /// </summary>
    public enum VersionCompareValues
    {
        NewRelease,
        UpToDate,
        NeedsUpgrade,
        Error,
    }
}
