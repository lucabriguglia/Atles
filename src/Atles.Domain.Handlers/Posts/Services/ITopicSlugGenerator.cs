using System;
using System.Threading.Tasks;

namespace Atles.Domain.Handlers.Posts.Commands
{
    public interface ITopicSlugGenerator
    {
        Task<string> GenerateTopicSlug(Guid forumId, string title);
    }
}
