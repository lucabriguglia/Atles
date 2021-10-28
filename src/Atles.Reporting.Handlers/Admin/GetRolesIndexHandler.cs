using System.Threading.Tasks;
using Atles.Reporting.Models.Admin.Roles;
using Atles.Reporting.Models.Admin.Roles.Queries;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenCqrs.Queries;

namespace Atles.Reporting.Handlers.Admin
{
    public class GetRolesIndexHandler : IQueryHandler<GetRolesIndex, IndexPageModel>
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public GetRolesIndexHandler(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IndexPageModel> Handle(GetRolesIndex query)
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
