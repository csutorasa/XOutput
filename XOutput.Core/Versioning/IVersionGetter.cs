using System.Threading.Tasks;

namespace XOutput.Versioning
{
    public interface IVersionGetter
    {
        Task<string> GetLatestReleaseAsync();
    }
}
