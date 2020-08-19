using System;
using System.Threading.Tasks;

namespace Atlas.Models.Admin.Site
{
    public interface ISiteModelBuilder
    {
        Task<SettingsPageModel> BuildSettingsPageModelAsync(Guid siteId);
    }
}
