using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.UpdateChecker
{
    /// <summary>
    /// Version related informations.
    /// </summary>
    public static class Version
    {
        /// <summary>
        /// Current application version.
        /// </summary>
        public const string AppVersion = "3.4";

        public static VersionCompare Compare(string version)
        {
            try
            {
                var current = AppVersion.Split('.').Select(t => int.Parse(t)).ToArray();
                var compare = version.Split('.').Select(t => int.Parse(t)).ToArray();
                for (int i = 0; true; i++)
                {
                    bool currentNotPresent = i >= current.Length;
                    bool compareNotPresent = i >= compare.Length;
                    if (compareNotPresent)
                    {
                        if (currentNotPresent)
                            return VersionCompare.UpToDate;
                        else
                            return VersionCompare.NewRelease;
                    }
                    else
                    {
                        if (currentNotPresent)
                            return VersionCompare.NeedsUpgrade;
                        else
                        {
                            int currentValue = current[i];
                            int compareValue = compare[i];
                            if (currentValue > compareValue)
                                return VersionCompare.NewRelease;
                            if (currentValue < compareValue)
                                return VersionCompare.NeedsUpgrade;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return VersionCompare.Error;
            }
        }
    }

    public enum VersionCompare
    {
        NewRelease,
        UpToDate,
        NeedsUpgrade,
        Error,
    }
}
