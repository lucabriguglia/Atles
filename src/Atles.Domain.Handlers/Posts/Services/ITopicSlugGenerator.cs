using System;
using System.Threading.Tasks;

namespace Atles.Domain.Handlers.Posts.Services
{
    public interface ITopicSlugGenerator
    {
        Task<string> GenerateTopicSlug(Guid forumId, string title);
    }
}
