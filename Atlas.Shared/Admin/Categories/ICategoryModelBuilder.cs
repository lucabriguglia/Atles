using System;
using System.Threading.Tasks;
using Atlas.Shared.Admin.Categories.Models;

namespace Atlas.Shared.Admin.Categories
{
    public interface ICategoryModelBuilder
    {
        Task<IndexPageModel> BuildIndexPageModelAsync(Guid siteId);
        Task<FormComponentModel> BuildFormModelAsync(Guid siteId, Guid? id = null);
    }
}
