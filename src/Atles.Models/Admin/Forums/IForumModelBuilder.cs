using System;
using System.Threading.Tasks;

namespace Atles.Models.Admin.Forums
{
    public interface IForumModelBuilder
    {
        Task<IndexPageModel> BuildIndexPageModelAsync(Guid siteId, Guid? categoryId = null);
        Task<FormComponentModel> BuildCreateFormModelAsync(Guid siteId, Guid? categoryId = null);
        Task<FormComponentModel> BuildEditFormModelAsync(Guid siteId, Guid id);
    }
}
