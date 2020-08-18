using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Atlify.Server.Services
{
    public interface IIntegrityService
    {
        Task EnsureMemberCreatedAsync(IdentityUser user);
    }
}