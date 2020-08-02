using System.Threading.Tasks;
using Atlas.Domain.Members;
using Atlas.Domain.Sites;

namespace Atlas.Server.Services
{
    public interface IContextService
    {
        Task<Site> CurrentSiteAsync();
        Task<Member> CurrentMemberAsync();
    }
}