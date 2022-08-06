using Atles.Core.Queries;
using Atles.Models.Admin.Categories;

namespace Atles.Queries.Admin;

public record GetCategoriesIndex(Guid SiteId) : QueryRecordBase<CategoriesPageModel>(SiteId);
