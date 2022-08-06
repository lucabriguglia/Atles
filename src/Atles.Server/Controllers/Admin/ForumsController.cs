using Atles.Commands.Forums;
using Atles.Core;
using Atles.Domain;
using Atles.Models.Admin.Forums;
using Atles.Queries.Admin;
using Atles.Server.Mapping;
using Atles.Validators.ValidationRules;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Atles.Server.Controllers.Admin;

[Route("api/admin/forums")]
public class ForumsController : AdminControllerBase
{
    private readonly IForumValidationRules _forumValidationRules;
    private readonly IMapper<ForumFormModel.ForumModel, CreateForum> _createForumMapper;
    private readonly IMapper<ForumFormModel.ForumModel, UpdateForum> _updateForumMapper;
    private readonly IValidator<ForumFormModel.ForumModel> _forumValidator;

    public ForumsController(
        IDispatcher dispatcher,
        IForumValidationRules forumValidationRules,
        IMapper<ForumFormModel.ForumModel, CreateForum> createForumMapper,
        IMapper<ForumFormModel.ForumModel, UpdateForum> updateForumMapper, 
        IValidator<ForumFormModel.ForumModel> forumValidator) 
        : base(dispatcher)
    {
        _forumValidationRules = forumValidationRules;
        _createForumMapper = createForumMapper;
        _updateForumMapper = updateForumMapper;
        _forumValidator = forumValidator;
    }

    [HttpGet("index-model")]
    public async Task<ActionResult> Index() =>
        await ProcessGet(new GetForumsIndex(CurrentSite.Id));

    [HttpGet("index-model/{categoryId}")]
    public async Task<ActionResult> Index(Guid categoryId) =>
        await ProcessGet(new GetForumsIndex(CurrentSite.Id, categoryId));

    [HttpGet("create")]
    public async Task<ActionResult> Create() =>
        await ProcessGet(new GetForumCreateForm(CurrentSite.Id));

    [HttpGet("create/{categoryId}")]
    public async Task<ActionResult> Create(Guid categoryId) =>
        await ProcessGet(new GetForumCreateForm(CurrentSite.Id, categoryId));

    [HttpPost("save")]
    public async Task<ActionResult> Save(ForumFormModel.ForumModel model) => 
        await ProcessPost(model, _createForumMapper, _forumValidator);

    [HttpGet("edit/{id}")]
    public async Task<ActionResult<ForumFormModel>> Edit(Guid id) =>
        await ProcessGet(new GetForumEditForm(CurrentSite.Id, id));

    [HttpPost("update")]
    public async Task<ActionResult> Update(ForumFormModel.ForumModel model) => 
        await ProcessPost(model, _updateForumMapper, _forumValidator);

    [HttpPost("move-up")]
    public async Task<ActionResult> MoveUp([FromBody] Guid id) =>
        await ProcessPost(new MoveForum
        {
            ForumId = id,
            Direction = DirectionType.Up,
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id
        });

    [HttpPost("move-down")]
    public async Task<ActionResult> MoveDown([FromBody] Guid id) =>
        await ProcessPost(new MoveForum
        {
            ForumId = id,
            Direction = DirectionType.Down,
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id
        });

    [HttpDelete("delete/{id}")]
    public async Task<ActionResult> Delete(Guid id) =>
        await ProcessPost(new DeleteForum
        {
            ForumId = id,
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id
        });

    [HttpGet("is-name-unique/{categoryId}/{id}/{name}")]
    public async Task<IActionResult> IsNameUnique(Guid categoryId, string name, Guid id) => 
        Ok(await _forumValidationRules.IsForumNameUnique(CurrentSite.Id, categoryId, id, name));

    [HttpGet("is-slug-unique/{id}/{slug}")]
    public async Task<IActionResult> IsSlugUnique(string slug, Guid id) => 
        Ok(await _forumValidationRules.IsForumSlugUnique(CurrentSite.Id, id, slug));

    [HttpGet("is-forum-valid/{id}")]
    public async Task<IActionResult> IsForumValid(Guid id) => 
        Ok(await _forumValidationRules.IsForumValid(CurrentSite.Id, id));
}
