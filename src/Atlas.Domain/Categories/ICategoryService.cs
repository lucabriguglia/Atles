using System.Threading.Tasks;
using Atlas.Domain.Categories.Commands;

namespace Atlas.Domain.Categories
{
    public interface ICategoryService
    {
        Task CreateAsync(CreateCategory command);
        Task UpdateAsync(UpdateCategory command);
        Task MoveAsync(MoveCategory command);
        Task DeleteAsync(DeleteCategory command);
    }
}
