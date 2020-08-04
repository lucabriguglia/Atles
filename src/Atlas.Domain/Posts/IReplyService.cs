using System.Threading.Tasks;
using Atlas.Domain.Posts.Commands;

namespace Atlas.Domain.Posts
{
    public interface IReplyService
    {
        Task CreateAsync(CreateReply command);
        Task UpdateAsync(UpdateReply command);
        Task DeleteAsync(DeleteReply command);
    }
}
