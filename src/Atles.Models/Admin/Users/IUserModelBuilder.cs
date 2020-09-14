using System;
using System.Threading.Tasks;

namespace Atlas.Models.Admin.Users
{
    public interface IUserModelBuilder
    {
        Task<IndexPageModel> BuildIndexPageModelAsync(QueryOptions options, string status = null);
        Task<CreatePageModel> BuildCreatePageModelAsync();
        Task<EditPageModel> BuildEditPageModelAsync(Guid memberId);
        Task<EditPageModel> BuildEditPageModelAsync(string identityUserId);
        Task<ActivityPageModel> BuildActivityPageModelAsync(Guid siteId, Guid userId, QueryOptions options);
    }
}