using System.Threading.Tasks;
using Atles.Domain.Sites.Commands;

namespace Atles.Domain.Sites
{
    public interface ISiteService
    {
        Task UpdateAsync(UpdateSite command);
    }
}
