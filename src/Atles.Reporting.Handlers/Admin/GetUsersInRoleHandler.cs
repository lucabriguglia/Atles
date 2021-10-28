using System.Collections.Generic;
using System.Threading.Tasks;
using Atles.Reporting.Models.Admin.Roles;
using Atles.Reporting.Models.Admin.Roles.Queries;
using Microsoft.AspNetCore.Identity;
using OpenCqrs.Queries;

namespace Atles.Reporting.Handlers.Admin
{
    public class GetUsersInRoleHandler : IQueryHandler<GetUsersInRole, IList<IndexPageModel.UserModel>>
    {
        private readonly UserManager<IdentityUser> _userManager;

        public GetUsersInRoleHandler(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IList<IndexPageModel.UserModel>> Handle(GetUsersInRole query)
        {
            var result = new List<IndexPageModel.UserModel>();

            foreach (var user in await _userManager.GetUsersInRoleAsync(query.RoleName))
            {
                result.Add(new IndexPageModel.UserModel { Id = user.Id, Email = user.Email });
            }

            return result;
        }
    }
}
