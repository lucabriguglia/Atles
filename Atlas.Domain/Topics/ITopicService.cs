using Atlas.Domain.Topics.Commands;
using System.Threading.Tasks;

namespace Atlas.Domain.Topics
{
    public interface ITopicService
    {
        Task CreateAsync(CreateTopic command);
        Task UpdateAsync(UpdateTopic command);
        Task DeleteAsync(DeleteTopic command);
    }
}
