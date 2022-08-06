using Atles.Core.Queries;
using Atles.Models.Admin.PermissionSets;

namespace Atles.Queries.Admin;

public record GetPermissionSetEditForm(Guid SiteId, Guid Id) : QueryRecordBase<PermissionSetFormModel>(SiteId);
