using System.Threading.Tasks;
using Atlas.Server.Domain;

namespace Atlas.Server.Services
{
    public interface IContextService
    {
        Task<Site> CurrentSiteAsync();
        Task<Member> CurrentMemberAsync();
    }
}