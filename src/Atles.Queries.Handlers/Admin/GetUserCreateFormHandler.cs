using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Models.Admin.Users;
using Atles.Queries.Admin;

namespace Atles.Queries.Handlers.Admin;

public class GetUserCreateFormHandler : IQueryHandler<GetUserCreateForm, CreateUserPageModel>
{
    public async Task<QueryResult<CreateUserPageModel>> Handle(GetUserCreateForm request)
    {
        await Task.CompletedTask;

        var result = new CreateUserPageModel();

        return result;
    }
}
