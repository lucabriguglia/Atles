using System.Threading.Tasks;

namespace Atlas.Services
{
    public interface ISiteService
    {
        Task CreateAsync(string name);
    }
}
