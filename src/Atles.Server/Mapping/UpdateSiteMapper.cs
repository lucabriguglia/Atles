using Atles.Commands.Sites;
using Atles.Models.Admin.Sites;

namespace Atles.Server.Mapping;

public class UpdateSiteMapper : IMapper<SettingsPageModel.SiteModel, UpdateSite>
{
    public UpdateSite Map(SettingsPageModel.SiteModel model, Guid userId)
    {
        return new UpdateSite
        {
            Title = model.Title,
            Theme = model.Theme,
            Css = model.Css,
            Language = model.Language,
            Privacy = model.Privacy,
            Terms = model.Terms,
            HeadScript = model.HeadScript,
            SiteId = model.SiteId,
            UserId = userId
        };
    }
}
