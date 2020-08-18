using System.Threading.Tasks;
using Atlify.Domain.Posts.Commands;

namespace Atlify.Domain.Posts
{
    public interface IReplyService
    {
        Task CreateAsync(CreateReply command);
        Task UpdateAsync(UpdateReply command);
        Task DeleteAsync(DeleteReply command);
    }
}
