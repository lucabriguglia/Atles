using System;
using System.Threading.Tasks;
using Atlas.Shared.Admin.Forums.Models;

namespace Atlas.Shared.Admin.Forums
{
    public interface IForumModelBuilder
    {
        Task<IndexPageModel> BuildIndexPageModelAsync(Guid siteId, Guid? categoryId = null);
        Task<FormComponentModel> BuildCreateFormModelAsync(Guid siteId, Guid? categoryId = null);
        Task<FormComponentModel> BuildEditFormModelAsync(Guid siteId, Guid id);
    }
}
