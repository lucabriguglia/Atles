using System.Threading.Tasks;

namespace Atlas.Models.Admin.Roles
{
    public interface IRoleModelBuilder
    {
        Task<IndexPageModel> BuildIndexPageModelAsync();
    }
}
