using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atles.Core.Queries;
using Atles.Domain.Models;
using Atles.Reporting.Models.Admin.Roles;
using Atles.Reporting.Models.Admin.Roles.Queries;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atles.Reporting.Handlers.Admin
{
    public class GetRolesHandler : IQueryHandler<GetRoles, IList<RoleModel>>
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public GetRolesHandler(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IList<RoleModel>> Handle(GetRoles query)
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
