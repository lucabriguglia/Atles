using System.Threading.Tasks;
using Atlas.Domain.Members;
using Atlas.Models.Public;

namespace Atlas.Server.Services
{
    public interface IContextService
    {
        Task<SiteModel> CurrentSiteAsync();
        Task<Member> CurrentMemberAsync();
    }
}