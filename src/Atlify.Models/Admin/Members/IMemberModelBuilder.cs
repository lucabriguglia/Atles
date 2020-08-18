using System;
using System.Threading.Tasks;

namespace Atlify.Models.Admin.Members
{
    public interface IMemberModelBuilder
    {
        Task<IndexPageModel> BuildIndexPageModelAsync(QueryOptions options);
        Task<CreatePageModel> BuildCreatePageModelAsync();
        Task<EditPageModel> BuildEditPageModelAsync(Guid memberId);
        Task<EditPageModel> BuildEditPageModelAsync(string userId);
        Task<ActivityPageModel> BuildActivityPageModelAsync(Guid siteId, Guid memberId, QueryOptions options);
    }
}