using System.Threading.Tasks;
using Atles.Domain.Posts.Commands;

namespace Atles.Domain.Posts
{
    public interface IReplyService
    {
        Task CreateAsync(CreateReply command);
        Task UpdateAsync(UpdateReply command);
        Task SetAsAnswerAsync(SetReplyAsAnswer command);
        Task DeleteAsync(DeleteReply command);
    }
}
