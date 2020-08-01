using System;
using System.Threading.Tasks;

namespace Atlas.Models.Admin.Members
{
    public interface IMemberModelBuilder
    {
        Task<IndexPageModel> BuildIndexPageModelAsync();
        Task<FormComponentModel> BuildFormModelAsync(Guid? id = null);
    }
}
