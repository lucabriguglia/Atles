using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atles.Domain;
using Atles.Models.Admin.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atles.Data.Builders.Admin
{
    public class RoleModelBuilder : IRoleModelBuilder
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public RoleModelBuilder(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
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

        public async Task<IList<IndexPageModel.UserModel>> BuildUsersInRoleModelsAsync(string roleName)
        {
            var result = new List<IndexPageModel.UserModel>();

            foreach (var user in await _userManager.GetUsersInRoleAsync(roleName))
            {
                result.Add(new IndexPageModel.UserModel{Id = user.Id, Email = user.Email});
            }

            return result;
        }

        public async Task<IList<RoleModel>> GetRoleModelsAsync()
        {
            var result = new List<RoleModel>
            {
                new RoleModel
                {
                    Id = Consts.RoleIdAll, 
                    Name = Consts.RoleNameAll
                },
                new RoleModel
                {
                    Id = Consts.RoleIdRegistered, 
                    Name = Consts.RoleNameRegistered
                }
            };

            result.AddRange(from role in await _roleManager.Roles
                .OrderBy(x => x.Name).ToListAsync() 
                select new RoleModel
                {
                    Id = role.Id, 
                    Name = role.Name
                });

            return result;
        }
    }
}