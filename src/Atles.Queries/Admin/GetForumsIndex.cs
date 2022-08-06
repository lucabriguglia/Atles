using Atles.Core.Queries;
using Atles.Models.Admin.Forums;

namespace Atles.Queries.Admin;

public record GetForumsIndex(Guid SiteId, Guid? CategoryId = null) : QueryRecordBase<ForumsPageModel>(SiteId);
