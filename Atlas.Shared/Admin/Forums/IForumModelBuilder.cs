using System;
using System.Threading.Tasks;
using Atlas.Shared.Admin.Forums.Models;

namespace Atlas.Shared.Admin.Forums
{
    public interface IForumModelBuilder
    {
        Task<IndexModel> BuildIndexModelAsync(Guid siteId, Guid? forumGroupId = null);
        Task<FormModel> BuildCreateFormModelAsync(Guid siteId, Guid? forumGroupId = null);
        Task<FormModel> BuildEditFormModelAsync(Guid siteId, Guid id);
    }
}
