using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Atlas.Server.Services
{
    public interface IIntegrityService
    {
        Task EnsureMemberCreatedAsync(IdentityUser user);
    }
}