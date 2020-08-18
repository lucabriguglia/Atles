using System.Threading.Tasks;
using Atlify.Domain.Forums.Commands;

namespace Atlify.Domain.Forums
{
    public interface IForumService
    {
        Task CreateAsync(CreateForum command);
        Task UpdateAsync(UpdateForum command);
        Task MoveAsync(MoveForum command);
        Task DeleteAsync(DeleteForum command);
    }
}
