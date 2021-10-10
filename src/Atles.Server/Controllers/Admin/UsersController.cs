using System;
using System.Linq;
using System.Threading.Tasks;
using Atles.Domain.Users;
using Atles.Domain.Users.Commands;
using Atles.Models;
using Atles.Models.Admin.Users;
using Atles.Server.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenCqrs;

namespace Atles.Server.Controllers.Admin
{
    [Route("api/admin/users")]
    public class UsersController : AdminControllerBase
    {
        private readonly IContextService _contextService;
        private readonly IUserService _userService;
        private readonly IUserRules _userRules;
        private readonly IUserModelBuilder _modelBuilder;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ISender _sender;

        public UsersController(IContextService contextService,
            IUserService userService,
            IUserRules userRules,
            IUserModelBuilder modelBuilder, 
            UserManager<IdentityUser> userManager,
            ISender sender)
        {
            _contextService = contextService;
            _userService = userService;
            _userRules = userRules;
            _modelBuilder = modelBuilder;
            _userManager = userManager;
            _sender = sender;
        }

        [HttpGet("index-model")]
        public async Task<IndexPageModel> List(
            [FromQuery] int? page = 1, 
            [FromQuery] string search = null, 
            [FromQuery] string status = null,
            [FromQuery] string sortByField = null,
            [FromQuery] string sortByDirection = null)
        {
            return await _modelBuilder.BuildIndexPageModelAsync(new QueryOptions(page, search, sortByField, sortByDirection), status);
        }

        [HttpGet("create")]
        public async Task<CreatePageModel> Create()
        {
            return await _modelBuilder.BuildCreatePageModelAsync();
        }

        [HttpPost("save")]
        public async Task<ActionResult> Save(CreatePageModel.UserModel model)
        {
            if (!ModelState.IsValid) return BadRequest();

            var identityUser = new IdentityUser { UserName = model.Email, Email = model.Email };
            var createResult = await _userManager.CreateAsync(identityUser, model.Password);

            if (!createResult.Succeeded) return BadRequest();

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
            var confirmResult = await _userManager.ConfirmEmailAsync(identityUser, code);

            var site = await _contextService.CurrentSiteAsync();
            var user = await _contextService.CurrentUserAsync();

            var command = new CreateUser
            {
                IdentityUserId = identityUser.Id,
                Email = identityUser.Email,
                SiteId = site.Id,
                UserId = user.Id,
                Confirm = true
            };

            await _userService.CreateAsync(command);

            return Ok(command.Id);
        }

        [HttpGet("edit/{id}")]
        public async Task<ActionResult<EditPageModel>> Edit(Guid id)
        {
            var result = await _modelBuilder.BuildEditPageModelAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        [HttpGet("edit-by-identity-user-id/{id}")]
        public async Task<ActionResult<EditPageModel>> EditByIdentityUserId(string identityUserId)
        {
            var result = await _modelBuilder.BuildEditPageModelAsync(identityUserId);

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        [HttpPost("update")]
        public async Task<ActionResult> Update(EditPageModel model)
        {
            var site = await _contextService.CurrentSiteAsync();
            var user = await _contextService.CurrentUserAsync();

            var identityUser = await _userManager.FindByIdAsync(model.Info.UserId);

            if (identityUser != null && model.Roles.Count > 0)
            {
                foreach (var role in model.Roles)
                {
                    if (role.Selected)
                    {
                        if (!await _userManager.IsInRoleAsync(identityUser, role.Name))
                        {
                            await _userManager.AddToRoleAsync(identityUser, role.Name);
                        }
                    }
                    else
                    {
                        if (await _userManager.IsInRoleAsync(identityUser, role.Name))
                        {
                            await _userManager.RemoveFromRoleAsync(identityUser, role.Name);
                        }
                    }
                }
            }

            var command = new UpdateUser
            {
                Id = model.User.Id,
                DisplayName = model.User.DisplayName,
                SiteId = site.Id,
                UserId = user.Id,
                Roles = model.Roles.Where(x => x.Selected).Select(x => x.Name).ToList()
            };

            await _userService.UpdateAsync(command);

            return Ok();
        }

        [HttpGet("activity/{id}")]
        public async Task<ActionResult<ActivityPageModel>> Activity(Guid id, [FromQuery] int? page = 1, [FromQuery] string search = null)
        {
            var site = await _contextService.CurrentSiteAsync();

            var result = await _modelBuilder.BuildActivityPageModelAsync(site.Id, id, new QueryOptions(page, search));

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        [HttpPost("suspend")]
        public async Task<ActionResult> Suspend([FromBody] Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();
            var user = await _contextService.CurrentUserAsync();

            var command = new SuspendUser
            {
                Id = id,
                SiteId = site.Id,
                UserId = user.Id
            };

            await _userService.SuspendAsync(command);

            return Ok();
        }

        [HttpPost("reinstate")]
        public async Task<ActionResult> Reinstate([FromBody] Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();
            var user = await _contextService.CurrentUserAsync();

            var command = new ReinstateUser
            {
                Id = id,
                SiteId = site.Id,
                UserId = user.Id
            };

            await _userService.ReinstateAsync(command);

            return Ok();
        }

        [HttpDelete("delete/{id}/{identityUserId}")]
        public async Task<ActionResult> Delete(Guid id, string identityUserId)
        {
            var site = await _contextService.CurrentSiteAsync();
            var user = await _contextService.CurrentUserAsync();

            var command = new DeleteUser
            {
                Id = id,
                IdentityUserId = identityUserId,
                SiteId = site.Id,
                UserId = user.Id
            };

            await _userService.DeleteAsync(command);

            var identityUser = await _userManager.FindByIdAsync(identityUserId);

            if (identityUser != null)
            {
                await _userManager.DeleteAsync(identityUser);
            }

            return Ok();
        }

        [HttpGet("is-display-name-unique/{name}")]
        public async Task<IActionResult> IsDisplayNameUnique(string name)
        {
            var isNameUnique = await _userRules.IsDisplayNameUniqueAsync(name);
            return Ok(isNameUnique);
        }

        [HttpGet("is-display-name-unique/{name}/{id}")]
        public async Task<IActionResult> IsNameUnique(string name, Guid id)
        {
            var isNameUnique = await _userRules.IsDisplayNameUniqueAsync(name, id);
            return Ok(isNameUnique);
        }
    }
}
