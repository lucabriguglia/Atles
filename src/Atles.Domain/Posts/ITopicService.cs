using System.Threading.Tasks;
using Atles.Domain.Posts.Commands;

namespace Atles.Domain.Posts
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
