using Atlas.Shared.Site.Models;
using System;
using System.Threading.Tasks;

namespace Atlas.Shared.Site
{
    public interface ISiteModelBuilder
    {
        Task<IndexPageModel> BuildIndexPageModelAsync(Guid siteId);
        Task<ForumPageModel> BuildForumPageModelAsync(Guid siteId, Guid forumId);
    }
}
