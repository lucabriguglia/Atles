using Atles.Core.Queries;
using Atles.Models.Admin.Categories;

namespace Atles.Queries.Admin;

public record GetCategoryForm(Guid SiteId, Guid? Id = null) : QueryRecordBase<CategoryFormModel>(SiteId);
