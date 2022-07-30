using System.Threading.Tasks;
using Atles.Core.Queries;
using Atles.Models.Admin.Users;
using Atles.Queries.Admin;

namespace Atles.Queries.Handlers.Admin
{
    public class GetUserCreateFormHandler : IQueryHandler<GetUserCreateForm, CreatePageModel>
    {
        public async Task<CreatePageModel> Handle(GetUserCreateForm request)
        {
            await Task.CompletedTask;

            var result = new CreatePageModel();

            return result;
        }
    }
}
