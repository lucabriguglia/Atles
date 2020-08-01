using Atlas.Domain.Sites.Commands;
using System.Threading.Tasks;

namespace Atlas.Domain.Sites
{
    public interface ISiteService
    {
        Task UpdateAsync(UpdateSite command);
    }
}
