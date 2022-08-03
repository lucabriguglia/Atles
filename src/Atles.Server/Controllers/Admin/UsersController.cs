﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Atles.Commands.Users;
using Atles.Core;
using Atles.Models;
using Atles.Models.Admin.Users;
using Atles.Queries.Admin;
using Atles.Validators.ValidationRules;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Atles.Server.Controllers.Admin;

[Route("api/admin/users")]
public class UsersController : AdminControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IDispatcher _dispatcher;
    private readonly IUserValidationRules _userValidationRules;

    public UsersController(
        UserManager<IdentityUser> userManager,
        IDispatcher dispatcher, 
        IUserValidationRules userValidationRules) 
        : base(dispatcher)
    {
        _userManager = userManager;
        _dispatcher = dispatcher;
        _userValidationRules = userValidationRules;
    }

    [HttpGet("index-model")]
    public async Task<ActionResult> List(
        [FromQuery] int? page = 1, 
        [FromQuery] string search = null, 
        [FromQuery] string status = null,
        [FromQuery] string sortByField = null,
        [FromQuery] string sortByDirection = null)
    {
        return await ProcessGet(new GetUsersIndex
        {
            Options = new QueryOptions(page, search, sortByField, sortByDirection), 
            Status = status
        });
    }

    [HttpGet("create")]
    public async Task<ActionResult> Create()
    {
        return await ProcessGet(new GetUserCreateForm());
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

        var command = new CreateUser
        {
            IdentityUserId = identityUser.Id,
            Email = identityUser.Email,
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id,
            Confirm = true
        };

        await _dispatcher.Send(command);

        return Ok(command.CreateUserId);
    }

    [HttpGet("edit/{id}")]
    public async Task<ActionResult<EditPageModel>> Edit(Guid id)
    {
        return await ProcessGet(new GetUserEditForm
        {
            Id = id
        });
    }

    [HttpGet("edit-by-identity-user-id/{identityUserId}")]
    public async Task<ActionResult<EditPageModel>> EditByIdentityUserId(string identityUserId)
    {
        return await ProcessGet(new GetUserEditForm
        {
            IdentityUserId = identityUserId 
        });
    }

    [HttpPost("update")]
    public async Task<ActionResult> Update(EditPageModel model)
    {
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
            UpdateUserId = model.User.Id,
            DisplayName = model.User.DisplayName,
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id,
            Roles = model.Roles.Where(x => x.Selected).Select(x => x.Name).ToList()
        };

        await _dispatcher.Send(command);

        return Ok();
    }

    [HttpGet("activity/{id}")]
    public async Task<ActionResult> Activity(Guid id, [FromQuery] int? page = 1, [FromQuery] string search = null)
    {
        return await ProcessGet(new GetUserActivity
        {
            Options = new QueryOptions(page, search), 
            SiteId = CurrentSite.Id, 
            UserId = id
        });
    }

    [HttpPost("suspend")]
    public async Task<ActionResult> Suspend([FromBody] Guid id)
    {
        var command = new SuspendUser
        {
            SuspendUserId = id,
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id
        };

        await _dispatcher.Send(command);

        return Ok();
    }

    [HttpPost("reinstate")]
    public async Task<ActionResult> Reinstate([FromBody] Guid id)
    {
        var command = new ReinstateUser
        {
            ReinstateUserId = id,
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id
        };

        await _dispatcher.Send(command);

        return Ok();
    }

    [HttpDelete("delete/{id}/{identityUserId}")]
    public async Task<ActionResult> Delete(Guid id, string identityUserId)
    {
        var command = new DeleteUser
        {
            DeleteUserId = id,
            IdentityUserId = identityUserId,
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id
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
    public async Task<ActionResult> IsDisplayNameUnique(string name)
    {
        var isDisplayNameUnique = await _userValidationRules.IsUserDisplayNameUnique(name);
        return Ok(isDisplayNameUnique);
    }

    [HttpGet("is-display-name-unique/{name}/{id}")]
    public async Task<ActionResult> IsNameUnique(string name, Guid id)
    {
        var isDisplayNameUnique = await _userValidationRules.IsUserDisplayNameUnique(name, id);
        return Ok(isDisplayNameUnique);
    }
}
