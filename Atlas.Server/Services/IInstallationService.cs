using System.Threading.Tasks;

namespace Atlas.Server.Services
{
    public interface IInstallationService
    {
        Task EnsureAdminUserInitializedAsync();
        Task EnsureDefaultSiteInitializedAsync();
    }
}