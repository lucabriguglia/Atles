using System;
using System.Linq;
using System.Threading.Tasks;
using Atles.Domain.Models.Users.Commands;
using Atles.Domain.Models.Users.Rules;
using Atles.Reporting.Models.Admin.Users;
using Atles.Reporting.Models.Admin.Users.Queries;
using Atles.Reporting.Models.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenCqrs;

namespace Atles.Server.Controllers.Admin
{
    [Route("api/admin/users")]
    public class UsersController : AdminControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IDispatcher _dispatcher;

        public UsersController(UserManager<IdentityUser> userManager,
            IDispatcher dispatcher) : base(dispatcher)
        {
            _userManager = userManager;
            _dispatcher = dispatcher;
        }

        [HttpGet("index-model")]
        public async Task<IndexPageModel> List(
            [FromQuery] int? page = 1, 
            [FromQuery] string search = null, 
            [FromQuery] string status = null,
            [FromQuery] string sortByField = null,
            [FromQuery] string sortByDirection = null)
        {
            var query = new GetUsersIndex { Options = new QueryOptions(page, search, sortByField, sortByDirection), Status = status };
            return await _dispatcher.Get(query);
        }

        [HttpGet("create")]
        public async Task<CreatePageModel> Create()
        {
            return await _dispatcher.Get(new GetUserCreateForm());
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

            var site = await CurrentSite();
            var user = await CurrentUser();

            var command = new CreateUser
            {
                IdentityUserId = identityUser.Id,
                Email = identityUser.Email,
                SiteId = site.Id,
                UserId = user.Id,
                Confirm = true
            };

            await _dispatcher.Send(command);

            return Ok(command.Id);
        }

        [HttpGet("edit/{id}")]
        public async Task<ActionResult<EditPageModel>> Edit(Guid id)
        {
            var result = await _dispatcher.Get(new GetUserEditForm { Id = id });

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        [HttpGet("edit-by-identity-user-id/{identityUserId}")]
        public async Task<ActionResult<EditPageModel>> EditByIdentityUserId(string identityUserId)
        {
            var result = await _dispatcher.Get(new GetUserEditForm { IdentityUserId = identityUserId });

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        [HttpPost("update")]
        public async Task<ActionResult> Update(EditPageModel model)
        {
            var site = await CurrentSite();
            var user = await CurrentUser();

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

            await _dispatcher.Send(command);

            return Ok();
        }

        [HttpGet("activity/{id}")]
        public async Task<ActionResult<ActivityPageModel>> Activity(Guid id, [FromQuery] int? page = 1, [FromQuery] string search = null)
        {
            var site = await CurrentSite();

            var query = new GetUserActivity { Options = new QueryOptions(page, search), SiteId = site.Id, UserId = id  };
            var result = await _dispatcher.Get(query);

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        [HttpPost("suspend")]
        public async Task<ActionResult> Suspend([FromBody] Guid id)
        {
            var site = await CurrentSite();
            var user = await CurrentUser();

            var command = new SuspendUser
            {
                Id = id,
                SiteId = site.Id,
                UserId = user.Id
            };

            await _dispatcher.Send(command);

            return Ok();
        }

        [HttpPost("reinstate")]
        public async Task<ActionResult> Reinstate([FromBody] Guid id)
        {
            var site = await CurrentSite();
            var user = await CurrentUser();

            var command = new ReinstateUser
            {
                Id = id,
                SiteId = site.Id,
                UserId = user.Id
            };

            await _dispatcher.Send(command);

            return Ok();
        }

        [HttpDelete("delete/{id}/{identityUserId}")]
        public async Task<ActionResult> Delete(Guid id, string identityUserId)
        {
            var site = await CurrentSite();
            var user = await CurrentUser();

            var command = new DeleteUser
            {
                Id = id,
                IdentityUserId = identityUserId,
                SiteId = site.Id,
                UserId = user.Id
            };

            await _dispatcher.Send(command);

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
            var isNameUnique = await _dispatcher.Get(new IsUserDisplayNameUnique { DisplayName = name });
            return Ok(isNameUnique);
        }

        [HttpGet("is-display-name-unique/{name}/{id}")]
        public async Task<IActionResult> IsNameUnique(string name, Guid id)
        {
            var isNameUnique = await _dispatcher.Get(new IsUserDisplayNameUnique { DisplayName = name, Id = id });
            return Ok(isNameUnique);
        }
    }
}