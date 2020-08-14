using System.Threading.Tasks;
using Atlas.Domain.Posts.Commands;

namespace Atlas.Domain.Posts
{
    public interface ITopicService
    {
        Task CreateAsync(CreateTopic command);
        Task UpdateAsync(UpdateTopic command);
        Task PinAsync(PinTopic command);
        Task LockAsync(LockTopic command);
        Task DeleteAsync(DeleteTopic command);
    }
}
