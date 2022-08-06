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
    private readonly IMapper<ForumFormModelBase.ForumModel, CreateForum> _createForumMapper;
    private readonly IValidator<ForumFormModelBase.ForumModel> _createForumValidator;
    private readonly IMapper<ForumFormModelBase.ForumModel, UpdateForum> _updateForumMapper;
    private readonly IValidator<ForumFormModelBase.ForumModel> _updateForumValidator;

    public ForumsController(
        IDispatcher dispatcher,
        IForumValidationRules forumValidationRules,
        IMapper<ForumFormModelBase.ForumModel, CreateForum> createForumMapper,
        IValidator<ForumFormModelBase.ForumModel> createForumValidator, 
        IMapper<ForumFormModelBase.ForumModel, UpdateForum> updateForumMapper, 
        IValidator<ForumFormModelBase.ForumModel> updateForumValidator) 
        : base(dispatcher)
    {
        _forumValidationRules = forumValidationRules;
        _createForumMapper = createForumMapper;
        _createForumValidator = createForumValidator;
        _updateForumMapper = updateForumMapper;
        _updateForumValidator = updateForumValidator;
    }

    [HttpGet("index-model")]
    public async Task<ActionResult> Index() =>
        await ProcessGet(new GetForumsIndex { SiteId = CurrentSite.Id });

    [HttpGet("index-model/{categoryId}")]
    public async Task<ActionResult> Index(Guid categoryId) =>
        await ProcessGet(new GetForumsIndex { SiteId = CurrentSite.Id, CategoryId = categoryId });

    [HttpGet("create")]
    public async Task<ActionResult> Create() =>
        await ProcessGet(new GetForumCreateForm { SiteId = CurrentSite.Id });

    [HttpGet("create/{categoryId}")]
    public async Task<ActionResult> Create(Guid categoryId) =>
        await ProcessGet(new GetForumCreateForm { SiteId = CurrentSite.Id, CategoryId = categoryId });

    [HttpPost("save")]
    public async Task<ActionResult> Save(ForumFormModelBase.ForumModel model) => 
        await ProcessPost(model, _createForumMapper, _createForumValidator);

    [HttpGet("edit/{id}")]
    public async Task<ActionResult<CreateForumFormModel>> Edit(Guid id) =>
        await ProcessGet(new GetForumEditForm { SiteId = CurrentSite.Id, Id = id });

    [HttpPost("update")]
    public async Task<ActionResult> Update(ForumFormModelBase.ForumModel model) => 
        await ProcessPost(model, _updateForumMapper, _updateForumValidator);

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

    [HttpGet("is-name-unique/{categoryId}/{name}")]
    public async Task<IActionResult> IsNameUnique(Guid categoryId, string name) => 
        Ok(await _forumValidationRules.IsForumNameUnique(CurrentSite.Id, categoryId, name));

    [HttpGet("is-name-unique/{categoryId}/{name}/{id}")]
    public async Task<IActionResult> IsNameUnique(Guid categoryId, string name, Guid id) => 
        Ok(await _forumValidationRules.IsForumNameUnique(CurrentSite.Id, categoryId, name, id));

    [HttpGet("is-slug-unique/{slug}")]
    public async Task<IActionResult> IsSlugUnique(string slug) => 
        Ok(await _forumValidationRules.IsForumSlugUnique(CurrentSite.Id, slug));

    [HttpGet("is-slug-unique/{slug}/{id}")]
    public async Task<IActionResult> IsSlugUnique(string slug, Guid id) => 
        Ok(await _forumValidationRules.IsForumSlugUnique(CurrentSite.Id, slug, id));

    [HttpGet("is-forum-valid/{id}")]
    public async Task<IActionResult> IsForumValid(Guid id) => 
        Ok(await _forumValidationRules.IsForumValid(CurrentSite.Id, id));
}
