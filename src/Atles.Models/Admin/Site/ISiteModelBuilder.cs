using System;
using System.Threading.Tasks;

namespace Atles.Models.Admin.Site
{
    public interface ISiteModelBuilder
    {
        Task<SettingsPageModel> BuildSettingsPageModelAsync(Guid siteId);
    }
}
