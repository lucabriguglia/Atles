using System.Threading.Tasks;

namespace Atles.Server.Services
{
    public interface IInstallationService
    {
        Task EnsureDefaultSiteInitializedAsync();
        Task InstallThemeAsync();
    }
}