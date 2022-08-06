using Atles.Core.Queries;
using Atles.Models.Admin.Sites;

namespace Atles.Queries.Admin;

public record GetSettingsPageModel(Guid SiteId) : QueryRecordBase<SettingsPageModel>(SiteId);
