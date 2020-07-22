using Atlas.Domain.Forums.Commands;
using System.Threading.Tasks;

namespace Atlas.Domain.ForumGroups
{
    public interface IForumService
    {
        Task CreateAsync(CreateForum command);
        Task UpdateAsync(UpdateForum command);
        Task MoveAsync(MoveForum command);
        Task DeleteAsync(DeleteForum command);
    }
}
