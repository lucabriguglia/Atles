using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Atles.Server.Services
{
    public interface IIdentityIntegrityService
    {
        Task EnsureUserCreatedAsync(IdentityUser identityUser, bool confirm = false);
        Task ConfirmUserAsync(IdentityUser identityUser);
        Task UpdateEmailAsync(IdentityUser identityUser);
    }
}