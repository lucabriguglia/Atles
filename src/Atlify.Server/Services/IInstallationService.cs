using System.Threading.Tasks;

namespace Atlify.Server.Services
{
    public interface IInstallationService
    {
        Task EnsureDefaultSiteInitializedAsync();
    }
}