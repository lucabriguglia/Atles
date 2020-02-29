using Atlas.Models;

namespace Atlas.Services
{
    public interface IContextService
    {
        Site CurrentSite();
        Member CurrentMember();
    }
}