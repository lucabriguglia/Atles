using System;
using System.Threading.Tasks;

namespace Atlas.Models.Admin.PermissionSets
{
    public interface IPermissionSetModelBuilder
    {
        Task<IndexPageModel> BuildIndexPageModelAsync(Guid siteId);
        Task<FormComponentModel> BuildFormModelAsync(Guid siteId, Guid? id = null);
    }
}
