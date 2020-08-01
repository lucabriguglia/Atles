using System.Threading.Tasks;
using Atlas.Domain.Members.Commands;

namespace Atlas.Domain.Members
{
    public interface IMemberService
    {
        Task CreateAsync(CreateMember command);
        Task UpdateAsync(UpdateMember command);
        Task DeleteAsync(DeleteMember command);
    }
}
