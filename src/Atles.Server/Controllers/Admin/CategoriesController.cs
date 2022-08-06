using Atles.Commands.Categories;
using Atles.Core;
using Atles.Domain;
using Atles.Models.Admin.Categories;
using Atles.Queries.Admin;
using Atles.Server.Mapping;
using Atles.Validators.ValidationRules;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Atles.Server.Controllers.Admin;

[Route("api/admin/categories")]
public class CategoriesController : AdminControllerBase
{
    private readonly ICategoryValidationRules _categoryValidationRules;
    private readonly IMapper<CreateCategoryFormModel.CategoryModel, CreateCategory> _createCategoryMapper;
    private readonly IValidator<CreateCategoryFormModel.CategoryModel> _createCategoryValidator;
    private readonly IMapper<UpdateCategoryFormModel.CategoryModel, UpdateCategory> _updateCategoryMapper;
    private readonly IValidator<UpdateCategoryFormModel.CategoryModel> _updateCategoryValidator;

    public CategoriesController(
        IDispatcher dispatcher,
        ICategoryValidationRules categoryValidationRules,
        IMapper<CreateCategoryFormModel.CategoryModel, CreateCategory> createCategoryMapper,
        IValidator<CreateCategoryFormModel.CategoryModel> createCategoryValidator, 
        IMapper<UpdateCategoryFormModel.CategoryModel, UpdateCategory> updateCategoryMapper, 
        IValidator<UpdateCategoryFormModel.CategoryModel> updateCategoryValidator) 
        : base(dispatcher)
    {
        _categoryValidationRules = categoryValidationRules;
        _createCategoryValidator = createCategoryValidator;
        _updateCategoryMapper = updateCategoryMapper;
        _updateCategoryValidator = updateCategoryValidator;
        _createCategoryMapper = createCategoryMapper;
    }

    [HttpGet("list")]
    public async Task<ActionResult> List() => 
        await ProcessGet(new GetCategoriesIndex { SiteId = CurrentSite.Id });

    [HttpGet("create")]
    public async Task<ActionResult> Create() => 
        await ProcessGet(new GetCategoryForm { SiteId = CurrentSite.Id });

    [HttpPost("save")]
    public async Task<ActionResult> Save(CreateCategoryFormModel.CategoryModel model) => 
        await ProcessPost(model, _createCategoryMapper, _createCategoryValidator);

    [HttpGet("edit/{id}")]
    public async Task<ActionResult> Edit(Guid id) => 
        await ProcessGet(new GetCategoryForm { SiteId = CurrentSite.Id, Id = id });

    [HttpPost("update")]
    public async Task<ActionResult> Update(UpdateCategoryFormModel.CategoryModel model) => 
        await ProcessPost(model, _updateCategoryMapper, _updateCategoryValidator);

    [HttpPost("move-up")]
    public async Task<ActionResult> MoveUp([FromBody] Guid id) =>
        await ProcessPost(new MoveCategory
        {
            CategoryId = id,
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id,
            Direction = DirectionType.Up
        });

    [HttpPost("move-down")]
    public async Task<ActionResult> MoveDown([FromBody] Guid id) =>
        await ProcessPost(new MoveCategory
        {
            CategoryId = id,
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id,
            Direction = DirectionType.Down
        });

    [HttpDelete("delete/{id}")]
    public async Task<ActionResult> Delete(Guid id) =>
        await ProcessPost(new DeleteCategory
        {
            CategoryId = id,
            SiteId = CurrentSite.Id,
            UserId = CurrentUser.Id
        });

    [HttpGet("is-name-unique/{name}")]
    public async Task<ActionResult> IsNameUnique(string name) => 
        Ok(await _categoryValidationRules.IsCategoryNameUnique(CurrentSite.Id, name));

    [HttpGet("is-name-unique/{name}/{id}")]
    public async Task<ActionResult> IsNameUnique(string name, Guid id) => 
        Ok(await _categoryValidationRules.IsCategoryNameUnique(CurrentSite.Id, name, id));
}
