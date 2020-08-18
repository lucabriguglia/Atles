using System;
using System.Threading.Tasks;

namespace Atlify.Models.Admin.Categories
{
    public interface ICategoryModelBuilder
    {
        Task<IndexPageModel> BuildIndexPageModelAsync(Guid siteId);
        Task<FormComponentModel> BuildFormModelAsync(Guid siteId, Guid? id = null);
    }
}
