using System;
using System.Threading.Tasks;

namespace Atlas.Domain.Users
{
    public interface IUserRules
    {
        Task<bool> IsDisplayNameUniqueAsync(string displayName);
        Task<bool> IsDisplayNameUniqueAsync(string displayName, Guid id);
    }
}
