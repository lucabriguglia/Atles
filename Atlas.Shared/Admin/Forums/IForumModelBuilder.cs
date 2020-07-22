using Atlas.Shared.Models.Admin.Forums;
using System;
using System.Threading.Tasks;

namespace Atlas.Shared.Forums
{
    public interface IForumModelBuilder
    {
        Task<IndexModel> BuildIndexModelAsync(Guid siteId, Guid? forumGroupId = null);
        Task<FormModel> BuildCreateFormModelAsync(Guid siteId, Guid forumGroupId);
        Task<FormModel> BuildEditFormModelAsync(Guid siteId, Guid id);
    }
}
