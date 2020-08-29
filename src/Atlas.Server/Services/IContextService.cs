using System.Collections.Generic;
using System.Threading.Tasks;
using Atlas.Models.Public;

namespace Atlas.Server.Services
{
    public interface IContextService
    {
        Task<CurrentSiteModel> CurrentSiteAsync();
        Task<CurrentUserModel> CurrentUserAsync();
        Task<IList<CurrentForumModel>> CurrentForumsAsync();
    }
}