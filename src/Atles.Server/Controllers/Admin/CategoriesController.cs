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
    private readonly IMapper<CategoryFormModel.CategoryModel, CreateCategory> _createCategoryMapper;
    private readonly IMapper<CategoryFormModel.CategoryModel, UpdateCategory> _updateCategoryMapper;
    private readonly IValidator<CategoryFormModel.CategoryModel> _categoryValidator;

    public CategoriesController(
        IDispatcher dispatcher,
        ICategoryValidationRules categoryValidationRules,
        IMapper<CategoryFormModel.CategoryModel, CreateCategory> createCategoryMapper,
        IMapper<CategoryFormModel.CategoryModel, UpdateCategory> updateCategoryMapper, 
        IValidator<CategoryFormModel.CategoryModel> categoryValidator) 
        : base(dispatcher)
    {
        _categoryValidationRules = categoryValidationRules;
        _updateCategoryMapper = updateCategoryMapper;
        _categoryValidator = categoryValidator;
        _createCategoryMapper = createCategoryMapper;
    }

    [HttpGet("list")]
    public async Task<ActionResult> List() => 
        await ProcessGet(new GetCategoriesIndex(CurrentSite.Id));

    [HttpGet("create")]
    public async Task<ActionResult> Create() => 
        await ProcessGet(new GetCategoryForm(CurrentSite.Id));

    [HttpPost("save")]
    public async Task<ActionResult> Save(CategoryFormModel.CategoryModel model) => 
        await ProcessPost(model, _createCategoryMapper, _categoryValidator);

    [HttpGet("edit/{id}")]
    public async Task<ActionResult> Edit(Guid id) => 
        await ProcessGet(new GetCategoryForm(CurrentSite.Id, id));

    [HttpPost("update")]
    public async Task<ActionResult> Update(CategoryFormModel.CategoryModel model) => 
        await ProcessPost(model, _updateCategoryMapper, _categoryValidator);

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

    [HttpGet("is-name-unique/{id}/{name}")]
    public async Task<ActionResult> IsNameUnique(string name, Guid id) => 
        Ok(await _categoryValidationRules.IsCategoryNameUnique(CurrentSite.Id, id, name));
}
