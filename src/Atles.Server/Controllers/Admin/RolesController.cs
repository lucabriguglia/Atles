using System.Collections.Generic;
using System.Threading.Tasks;
using Atles.Domain;
using Atles.Models.Admin.Roles;
using Atles.Reporting.Admin.Roles.Queries;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenCqrs;

namespace Atles.Server.Controllers.Admin
{
    [Route("api/admin/roles")]
    public class RolesController : AdminControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ISender _sender;

        public RolesController(
            RoleManager<IdentityRole> roleManager, 
            UserManager<IdentityUser> userManager,
            ISender sender)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _sender = sender;
        }

        [HttpGet("list")]
        public async Task<IndexPageModel> List()
        {
            return await _sender.Send(new GetRolesIndex());
        }

        [HttpGet("users-in-role/{roleName}")]
        public async Task<IList<IndexPageModel.UserModel>> UsersInRole(string roleName)
        {
            return await _sender.Send(new GetUsersInRole { RoleName = roleName });
        }

        [HttpPost("create")]
        public async Task<ActionResult> Create(IndexPageModel.EditRoleModel model)
        {
            var identityRole = new IdentityRole(model.Name);

            await _roleManager.CreateAsync(identityRole);

            return Ok();
        }

        [HttpPost("update")]
        public async Task<ActionResult> Update(IndexPageModel.EditRoleModel model)
        {
            var identityRole = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Id == model.Id);

            if (identityRole == null)
            {
                return NotFound();
            }

            if (identityRole.Name == Consts.RoleNameAdmin)
            {
                return BadRequest();
            }

            identityRole.Name = model.Name;

            await _roleManager.UpdateAsync(identityRole);

            return Ok();
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var identityRole = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Id == id);

            if (identityRole == null)
            {
                return NotFound();
            }

            if (identityRole.Name == Consts.RoleNameAdmin)
            {
                return BadRequest();
            }

            await _roleManager.DeleteAsync(identityRole);

            return Ok();
        }

        [HttpDelete("remove-user-from-role/{userId}/{roleName}")]
        public async Task<ActionResult> RemoveUserFromRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            await _userManager.RemoveFromRoleAsync(user, roleName);

            return Ok();
        }
    }
}
