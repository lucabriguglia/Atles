using Atles.Core.Queries;
using Atles.Models.Admin.PermissionSets;

namespace Atles.Queries.Admin;

public record GetPermissionSetsIndex(Guid SiteId) : QueryRecordBase<PermissionSetsPageModel>(SiteId);
