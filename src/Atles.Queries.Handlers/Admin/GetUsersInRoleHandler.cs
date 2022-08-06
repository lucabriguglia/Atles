using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Models.Admin.Roles;
using Atles.Queries.Admin;
using Microsoft.AspNetCore.Identity;

namespace Atles.Queries.Handlers.Admin;

public class GetUsersInRoleHandler : IQueryHandler<GetUsersInRole, IList<IndexPageModel.UserModel>>
{
    private readonly UserManager<IdentityUser> _userManager;

    public GetUsersInRoleHandler(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<QueryResult<IList<IndexPageModel.UserModel>>> Handle(GetUsersInRole query)
    {
        var result = new List<IndexPageModel.UserModel>();

        foreach (var user in await _userManager.GetUsersInRoleAsync(query.RoleName))
        {
            result.Add(new IndexPageModel.UserModel { Id = user.Id, Email = user.Email });
        }

        return result;
    }
}
