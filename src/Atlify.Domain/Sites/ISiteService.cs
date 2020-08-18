using System.Threading.Tasks;
using Atlify.Domain.Sites.Commands;

namespace Atlify.Domain.Sites
{
    public interface ISiteService
    {
        Task UpdateAsync(UpdateSite command);
    }
}
