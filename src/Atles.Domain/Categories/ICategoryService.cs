using System.Threading.Tasks;
using Atles.Domain.Categories.Commands;

namespace Atles.Domain.Categories
{
    public interface ICategoryService
    {
        Task CreateAsync(CreateCategory command);
        Task UpdateAsync(UpdateCategory command);
        Task MoveAsync(MoveCategory command);
        Task DeleteAsync(DeleteCategory command);
    }
}
