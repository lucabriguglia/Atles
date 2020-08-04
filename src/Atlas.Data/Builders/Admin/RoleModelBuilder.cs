using System.Threading.Tasks;
using Atlas.Models.Admin.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Data.Builders.Admin
{
    public class RoleModelBuilder : IRoleModelBuilder
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleModelBuilder(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IndexPageModel> BuildIndexPageModelAsync()
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