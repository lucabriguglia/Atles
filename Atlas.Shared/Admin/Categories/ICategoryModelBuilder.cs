using System;
using System.Threading.Tasks;
using Atlas.Shared.Admin.Categories.Models;

namespace Atlas.Shared.Admin.Categories
{
    public interface ICategoryModelBuilder
    {
        Task<IndexModel> BuildIndexModelAsync(Guid siteId);
        Task<FormModel> BuildFormModelAsync(Guid siteId, Guid? id = null);
    }
}
