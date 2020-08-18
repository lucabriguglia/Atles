using System;
using System.Threading.Tasks;

namespace Atlify.Models.Public.Posts
{
    public interface IPostModelBuilder
    {
        Task<PostPageModel> BuildNewPostPageModelAsync(Guid siteId, Guid forumId);
        Task<PostPageModel> BuildEditPostPageModelAsync(Guid siteId, Guid forumId, Guid topicId);
    }
}