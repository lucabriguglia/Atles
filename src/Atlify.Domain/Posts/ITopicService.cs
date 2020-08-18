using System.Threading.Tasks;
using Atlify.Domain.Posts.Commands;

namespace Atlify.Domain.Posts
{
    public interface ITopicService
    {
        Task<string> CreateAsync(CreateTopic command);
        Task<string> UpdateAsync(UpdateTopic command);
        Task PinAsync(PinTopic command);
        Task LockAsync(LockTopic command);
        Task DeleteAsync(DeleteTopic command);
    }
}
