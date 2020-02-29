using System.Threading.Tasks;

namespace Atlas.Services
{
    public interface IInstallationService
    {
        Task EnsureAdminUserInitializedAsync();
        Task EnsureDefaultSiteInitializedAsync();
    }
}