using System.Threading.Tasks;
using Atlify.Domain.Categories.Commands;

namespace Atlify.Domain.Categories
{
    public interface ICategoryService
    {
        Task CreateAsync(CreateCategory command);
        Task UpdateAsync(UpdateCategory command);
        Task MoveAsync(MoveCategory command);
        Task DeleteAsync(DeleteCategory command);
    }
}
