using System.Threading.Tasks;
using Atlify.Domain.Members.Commands;

namespace Atlify.Domain.Members
{
    public interface IMemberService
    {
        Task CreateAsync(CreateMember command);
        Task UpdateAsync(UpdateMember command);
        Task SuspendAsync(SuspendMember command);
        Task ReinstateAsync(ReinstateMember command);
        Task<string> DeleteAsync(DeleteMember command);
        Task<string> GenerateDisplayNameAsync();
    }
}
