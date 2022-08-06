using Atles.Core.Queries;
using Atles.Models.Admin.Forums;

namespace Atles.Queries.Admin;

public record GetForumEditForm(Guid SiteId, Guid Id) : QueryRecordBase<ForumFormModel>(SiteId);
