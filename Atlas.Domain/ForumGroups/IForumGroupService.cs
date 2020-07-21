using Atlas.Domain.ForumGroups.Commands;
using System.Threading.Tasks;

namespace Atlas.Domain.ForumGroups
{
    public interface IForumGroupService
    {
        Task CreateAsync(CreateForumGroup command);
        Task UpdateAsync(UpdateForumGroup command);
        Task MoveAsync(MoveForumGroup command);
        Task DeleteAsync(DeleteForumGroup command);
    }
}
