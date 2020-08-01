using System.Threading.Tasks;
using Atlas.Domain;
using Atlas.Models.Admin.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Server.Controllers.Admin
{
    [Authorize(Policy = "Admin")]
    [Route("api/admin/roles")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleModelBuilder _roleModelBuilder;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(IRoleModelBuilder roleModelBuilder, RoleManager<IdentityRole> roleManager)
        {
            _roleModelBuilder = roleModelBuilder;
            _roleManager = roleManager;
        }

        [HttpGet("list")]
        public async Task<IndexPageModel> List()
        {
            return await _roleModelBuilder.BuildIndexPageModelAsync();
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
            // TODO: Check if role is in use

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
    }
}
