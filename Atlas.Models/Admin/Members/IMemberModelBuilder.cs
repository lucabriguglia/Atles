using System;
using System.Threading.Tasks;

namespace Atlas.Models.Admin.Members
{
    public interface IMemberModelBuilder
    {
        Task<IndexPageModel> BuildIndexPageModelAsync(PaginationOptions options);
        Task<CreatePageModel> BuildCreatePageModelAsync();
        Task<EditPageModel> BuildEditPageModelAsync(Guid id);
    }
}
