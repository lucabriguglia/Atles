using System;
using System.Threading.Tasks;

namespace Atlas.Models.Admin.Members
{
    public interface IMemberModelBuilder
    {
        Task<IndexPageModel> BuildIndexPageModelAsync(QueryOptions options);
        Task<CreatePageModel> BuildCreatePageModelAsync();
        Task<EditPageModel> BuildEditPageModelAsync(Guid id);
    }
}
