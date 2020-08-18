using System;
using System.Threading.Tasks;

namespace Atlify.Domain.Members
{
    public interface IMemberRules
    {
        Task<bool> IsDisplayNameUniqueAsync(string displayName);
        Task<bool> IsDisplayNameUniqueAsync(string displayName, Guid id);
    }
}
