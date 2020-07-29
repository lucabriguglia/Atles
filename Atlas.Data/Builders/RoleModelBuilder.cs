using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Domain;
using Atlas.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Data.Builders
{
    public class RoleModelBuilder : IRoleModelBuilder
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleModelBuilder(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IList<RoleModel>> GetRoleModels()
        {
            var result = new List<RoleModel>
            {
                new RoleModel {Id = Consts.RoleIdAll, Name = "All Users"},
                new RoleModel {Id = Consts.RoleIdAuthorized, Name = "Registered Users"},
                //new RoleModel {Id = Consts.RoleIdAnonymous, Name = "Anonymous Users"}
            };

            result.AddRange(from role in await _roleManager.Roles.ToListAsync() select new RoleModel { Id = role.Id, Name = role.Name });

            return result;
        }
    }
}