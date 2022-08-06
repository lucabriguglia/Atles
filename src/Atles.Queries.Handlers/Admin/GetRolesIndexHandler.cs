using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Models.Admin.Roles;
using Atles.Queries.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atles.Queries.Handlers.Admin
{
    public class GetRolesIndexHandler : IQueryHandler<GetRolesIndex, IndexPageModel>
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public GetRolesIndexHandler(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<QueryResult<IndexPageModel>> Handle(GetRolesIndex query)
        {
            var result = new IndexPageModel();

            foreach (var role in await _roleManager.Roles.ToListAsync())
            {
                result.Roles.Add(new IndexPageModel.RoleModel { Id = role.Id, Name = role.Name });
            }

            return result;
        }
    }
}
