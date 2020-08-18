using System.Collections.Generic;
using System.Threading.Tasks;
using Atlify.Models.Public;

namespace Atlify.Server.Services
{
    public interface IContextService
    {
        Task<CurrentSiteModel> CurrentSiteAsync();
        Task<CurrentMemberModel> CurrentMemberAsync();
        Task<IList<CurrentForumModel>> CurrentForumsAsync();
    }
}