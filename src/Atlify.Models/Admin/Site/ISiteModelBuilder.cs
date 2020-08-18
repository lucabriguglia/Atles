using System;
using System.Threading.Tasks;

namespace Atlify.Models.Admin.Site
{
    public interface ISiteModelBuilder
    {
        Task<SettingsPageModel> BuildSettingsPageModelAsync(Guid siteId);
    }
}
