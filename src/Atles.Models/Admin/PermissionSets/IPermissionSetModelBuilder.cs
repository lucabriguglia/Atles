using System;
using System.Threading.Tasks;

namespace Atles.Models.Admin.PermissionSets
{
    public interface IPermissionSetModelBuilder
    {
        Task<IndexPageModel> BuildIndexPageModelAsync(Guid siteId);
        Task<FormComponentModel> BuildCreateFormModelAsync(Guid siteId);
        Task<FormComponentModel> BuildEditFormModelAsync(Guid siteId, Guid id);
    }
}
