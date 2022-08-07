﻿using Atles.Commands.Users;
using Atles.Core;
using Atles.Models;
using Atles.Models.Admin.Users;
using Atles.Queries.Admin;
using Atles.Server.Mapping;
using Atles.Validators.ValidationRules;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Atles.Server.Controllers.Admin;

[Route("api/admin/users")]
public class UsersController : AdminControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IDispatcher _dispatcher;
    private readonly IUserValidationRules _userValidationRules;
    private readonly IValidator<CreatePageModel.UserModel> _createValidator;
    private readonly IMapper<CreatePageModel.UserModel, CreateUser> _createMapper;

    public UsersController(
        UserManager<IdentityUser> userManager,
        IDispatcher dispatcher, 
        IUserValidationRules userValidationRules, 
        IValidator<CreatePageModel.UserModel> createValidator, 
        IMapper<CreatePageModel.UserModel, CreateUser> createMapper) 
        : base(dispatcher)
    {
        _userManager = userManager;
        _dispatcher = dispatcher;
        _userValidationRules = userValidationRules;
        _createValidator = createValidator;
        _createMapper = createMapper;
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
    public async Task<ActionResult> Create() => 
        await ProcessGet(new GetUserCreateForm());

    [HttpPost("save")]
    public async Task<ActionResult> Save(CreatePageModel.UserModel model) => 
        await ProcessPost(model, _createMapper, _createValidator);

    [HttpGet("edit/{id}")]
    public async Task<ActionResult<EditPageModel>> Edit(Guid id) => 
        await ProcessGet(new GetUserEditForm{ Id = id });

    [HttpGet("edit-by-identity-user-id/{identityUserId}")]
    public async Task<ActionResult<EditPageModel>> EditByIdentityUserId(string identityUserId) => 
        await ProcessGet(new GetUserEditForm { IdentityUserId = identityUserId });

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
    public async Task<ActionResult> Suspend([FromBody] Guid id) =>
        await ProcessPost(new SuspendUser
        {
            SuspendUserId = id,
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id
        });

    [HttpPost("reinstate")]
    public async Task<ActionResult> Reinstate([FromBody] Guid id) =>
        await ProcessPost(new ReinstateUser
        {
            ReinstateUserId = id,
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id
        });

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

    [HttpGet("is-email-unique/{id}/{email}")]
    public async Task<ActionResult> IsEmailUnique(Guid id, string email) =>
        Ok(await _userValidationRules.IsUserEmailUnique(id, email));

    [HttpGet("is-display-name-unique/{id}/{displayName}")]
    public async Task<ActionResult> IsDisplayNameUnique(Guid id, string displayName) => 
        Ok(await _userValidationRules.IsUserDisplayNameUnique(id, displayName));
}
