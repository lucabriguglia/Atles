using Atles.Core.Queries;
using Atles.Models.Admin.Forums;

namespace Atles.Queries.Admin;

public record GetForumCreateForm(Guid SiteId, Guid? CategoryId = null) : QueryRecordBase<CreateForumFormModel>(SiteId);
