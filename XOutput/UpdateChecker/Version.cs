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
        public const string AppVersion = "3.2";

        public static VersionCompare Compare(string version)
        {
            try
            {
                var current = AppVersion.Split('.').Select(t => int.Parse(t)).ToArray();
                var compare = version.Split('.').Select(t => int.Parse(t)).ToArray();
                for (int i = 0; true; i++)
                {
                    if (i >= current.Length)
                    {
                        if (i >= compare.Length)
                            return VersionCompare.UpToDate;
                        else
                            return VersionCompare.NewRelease;
                    }
                    else
                    {
                        if (i >= compare.Length)
                            return VersionCompare.NeedsUpgrade;
                        else
                        {
                            if (current[i] > compare[i])
                                return VersionCompare.NewRelease;
                            if (current[i] < compare[i])
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
