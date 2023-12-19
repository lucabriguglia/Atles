using Atles.Core.Queries;
using Atles.Models;
using Atles.Models.Public;

namespace Atles.Queries.Public;

public record GetForumPage(Guid SiteId, string Slug, QueryOptions Options) : QueryRecordBase<ForumPageModel>(SiteId);
