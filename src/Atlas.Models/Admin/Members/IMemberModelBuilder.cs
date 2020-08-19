using System;
using System.Threading.Tasks;

namespace Atlas.Models.Admin.Members
{
    public interface IMemberModelBuilder
    {
        Task<IndexPageModel> BuildIndexPageModelAsync(QueryOptions options, string status = null);
        Task<CreatePageModel> BuildCreatePageModelAsync();
        Task<EditPageModel> BuildEditPageModelAsync(Guid memberId);
        Task<EditPageModel> BuildEditPageModelAsync(string userId);
        Task<ActivityPageModel> BuildActivityPageModelAsync(Guid siteId, Guid memberId, QueryOptions options);
    }
}