using Atles.Models.Admin.Roles;
using Microsoft.EntityFrameworkCore;
using OpenCqrs.Queries;
using System.Threading.Tasks;
using Atles.Reporting.Admin.Roles.Queries;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using Atles.Domain;
using System.Linq;

namespace Atles.Reporting.Handlers.Admin.Roles
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
