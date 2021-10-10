using System;
using System.Threading.Tasks;

namespace Atles.Models.Admin.Users
{
    public interface IUserModelBuilder
    {
        Task<CreatePageModel> BuildCreatePageModelAsync();
        Task<EditPageModel> BuildEditPageModelAsync(Guid userId);
        Task<EditPageModel> BuildEditPageModelAsync(string identityUserId);
        Task<ActivityPageModel> BuildActivityPageModelAsync(Guid siteId, Guid userId, QueryOptions options);
    }
}