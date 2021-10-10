using System;
using System.Threading.Tasks;

namespace Atles.Models.Public.Posts
{
    public interface IPostModelBuilder
    {
        Task<PostPageModel> BuildNewPostPageModelAsync(Guid siteId, Guid forumId);
        Task<PostPageModel> BuildEditPostPageModelAsync(Guid siteId, Guid forumId, Guid topicId);
    }
}