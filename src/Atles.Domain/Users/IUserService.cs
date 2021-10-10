using System.Threading.Tasks;
using Atles.Domain.Users.Commands;

namespace Atles.Domain.Users
{
    public interface IUserService
    {
        Task CreateAsync(CreateUser command);
        Task ConfirmAsync(ConfirmUser command);
        Task UpdateAsync(UpdateUser command);
        Task SuspendAsync(SuspendUser command);
        Task ReinstateAsync(ReinstateUser command);
        Task DeleteAsync(DeleteUser command);
        Task<string> GenerateDisplayNameAsync();
    }
}
