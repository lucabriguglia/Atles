using Atles.Commands.Users;
using Atles.Core;
using Atles.Models;
using Atles.Models.Admin.Users;
using Atles.Queries.Admin;
using Atles.Server.Mapping;
using Atles.Validators.ValidationRules;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Atles.Server.Controllers.Admin;

[Route("api/admin/users")]
public class UsersController : AdminControllerBase
{
    private readonly IUserValidationRules _userValidationRules;
    private readonly IValidator<CreateUserPageModel.UserModel> _createUserValidator;
    private readonly IMapper<CreateUserPageModel.UserModel, CreateUser> _createUserMapper;
    private readonly IValidator<EditUserPageModel.UserModel> _updateUserValidator;
    private readonly IMapper<EditUserPageModel.UserModel, UpdateUser> _updateUserMapper;

    public UsersController(
        IDispatcher dispatcher, 
        IUserValidationRules userValidationRules, 
        IValidator<CreateUserPageModel.UserModel> createUserValidator, 
        IMapper<CreateUserPageModel.UserModel, CreateUser> createUserMapper, 
        IValidator<EditUserPageModel.UserModel> updateUserValidator, 
        IMapper<EditUserPageModel.UserModel, UpdateUser> updateUserMapper) 
        : base(dispatcher)
    {
        _userValidationRules = userValidationRules;
        _createUserValidator = createUserValidator;
        _createUserMapper = createUserMapper;
        _updateUserValidator = updateUserValidator;
        _updateUserMapper = updateUserMapper;
    }

    [HttpGet("index-model")]
    public async Task<ActionResult> List(
        [FromQuery] int? page = 1, 
        [FromQuery] string search = null, 
        [FromQuery] string status = null,
        [FromQuery] string sortByField = null,
        [FromQuery] string sortByDirection = null) =>
        await ProcessGet(new GetUsersIndex
        {
            Options = new QueryOptions(page, search, sortByField, sortByDirection), 
            Status = status
        });

    [HttpGet("create")]
    public async Task<ActionResult> Create() => 
        await ProcessGet(new GetUserCreateForm());

    [HttpPost("save")]
    public async Task<ActionResult> Save(CreateUserPageModel.UserModel model) => 
        await ProcessPost(model, _createUserMapper, _createUserValidator);

    [HttpGet("edit/{id}")]
    public async Task<ActionResult<EditUserPageModel>> Edit(Guid id) => 
        await ProcessGet(new GetUserEditForm{ Id = id });

    [HttpGet("edit-by-identity-user-id/{identityUserId}")]
    public async Task<ActionResult<EditUserPageModel>> EditByIdentityUserId(string identityUserId) => 
        await ProcessGet(new GetUserEditForm { IdentityUserId = identityUserId });

    [HttpPost("update")]
    public async Task<ActionResult> Update(EditUserPageModel.UserModel model) => 
        await ProcessPost(model, _updateUserMapper, _updateUserValidator);

    [HttpGet("activity/{id}")]
    public async Task<ActionResult> Activity(
        Guid id, 
        [FromQuery] int? page = 1, 
        [FromQuery] string search = null) =>
        await ProcessGet(new GetUserActivity
        {
            Options = new QueryOptions(page, search), 
            SiteId = CurrentSite.Id, 
            UserId = id
        });

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
    public async Task<ActionResult> Delete(Guid id, string identityUserId) =>
        await ProcessPost(new DeleteUser
        {
            DeleteUserId = id,
            IdentityUserId = identityUserId,
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id
        });

    [HttpGet("is-email-unique/{id}/{email}")]
    public async Task<ActionResult> IsEmailUnique(Guid id, string email) =>
        Ok(await _userValidationRules.IsUserEmailUnique(id, email));

    [HttpGet("is-display-name-unique/{id}/{displayName}")]
    public async Task<ActionResult> IsDisplayNameUnique(Guid id, string displayName) => 
        Ok(await _userValidationRules.IsUserDisplayNameUnique(id, displayName));
}
