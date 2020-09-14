using System.Collections.Generic;
using System.Threading.Tasks;
using Atles.Models.Public;

namespace Atles.Server.Services
{
    public interface IContextService
    {
        Task<CurrentSiteModel> CurrentSiteAsync();
        Task<CurrentUserModel> CurrentUserAsync();
        Task<IList<CurrentForumModel>> CurrentForumsAsync();
    }
}