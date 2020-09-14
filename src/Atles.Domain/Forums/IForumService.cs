using System.Threading.Tasks;
using Atles.Domain.Forums.Commands;

namespace Atles.Domain.Forums
{
    public interface IForumService
    {
        Task CreateAsync(CreateForum command);
        Task UpdateAsync(UpdateForum command);
        Task MoveAsync(MoveForum command);
        Task DeleteAsync(DeleteForum command);
    }
}
