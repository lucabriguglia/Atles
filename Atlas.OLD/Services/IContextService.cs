using System.Threading.Tasks;
using Atlas.Domain;

namespace Atlas.Services
{
    public interface IContextService
    {
        Task<Site> CurrentSiteAsync();
        Task<Member> CurrentMemberAsync();
    }
}