using Atles.Models.Admin.Users;
using OpenCqrs.Queries;
using System.Threading.Tasks;
using Atles.Reporting.Admin.Users.Queries;

namespace Atles.Reporting.Handlers.Admin.Users
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
