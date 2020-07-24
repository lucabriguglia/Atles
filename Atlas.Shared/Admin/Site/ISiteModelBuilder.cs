using System;
using System.Threading.Tasks;
using Atlas.Shared.Admin.Site.Models;

namespace Atlas.Shared.Admin.Site
{
    public interface ISiteModelBuilder
    {
        Task<SettingsPageModel> BuildSettingsPageModelAsync(Guid siteId);
    }
}
