using System.Linq;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Reporting.Models.Admin.Sites;
using Atles.Reporting.Models.Admin.Sites.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OpenCqrs.Queries;

namespace Atles.Reporting.Handlers.Admin
{
    public class GetSettingsPageModelHandler : IQueryHandler<GetSettingsPageModel, SettingsPageModel>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public GetSettingsPageModelHandler(AtlesDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<SettingsPageModel> Handle(GetSettingsPageModel query)
        {
            var site = await _dbContext.Sites.FirstOrDefaultAsync(x => x.Id == query.SiteId);

            if (site == null)
            {
                return null;
            }

            var themes = _configuration["Themes"].Split(',');
            var css = _configuration["CSS"].Split(',');
            var languages = _configuration["Languages"].Split(',');

            return new SettingsPageModel
            {
                Themes = themes.ToList(),
                Css = css.ToList(),
                Languages = languages.ToList(),
                Site = new SettingsPageModel.SiteModel
                {
                    Id = site.Id,
                    Title = site.Title,
                    Theme = site.PublicTheme,
                    Css = site.PublicCss,
                    Language = site.Language,
                    Privacy = site.Privacy,
                    Terms = site.Terms,
                    HeadScript = site.HeadScript
                }
            };
        }
    }
}
