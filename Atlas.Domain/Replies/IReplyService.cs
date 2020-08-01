using System.Threading.Tasks;
using Atlas.Domain.Replies.Commands;

namespace Atlas.Domain.Replies
{
    public interface IReplyService
    {
        Task CreateAsync(CreateReply command);
        Task UpdateAsync(UpdateReply command);
        Task DeleteAsync(DeleteReply command);
    }
}
