using Atles.Core.Queries;
using Atles.Models.Admin.PermissionSets;

namespace Atles.Queries.Admin;

public record GetPermissionSetCreateForm(Guid SiteId) : QueryRecordBase<PermissionSetFormModel>(SiteId);
