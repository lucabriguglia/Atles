using Atlas.Shared.Models.Admin.ForumGroups;
using System;
using System.Threading.Tasks;

namespace Atlas.Shared
{
    public interface IForumGroupModelBuilder
    {
        Task<IndexModel> BuildIndexModelAsync(Guid siteId);
        Task<FormModel> BuildFormModelAsync(Guid siteId, Guid? id = null);
    }
}
