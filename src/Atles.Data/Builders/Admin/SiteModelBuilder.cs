using System;
using System.Linq;
using System.Threading.Tasks;
using Atles.Models.Admin.Site;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Atles.Data.Builders.Admin
{
    public class SiteModelBuilder : ISiteModelBuilder
    {
        private readonly AtlesDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public SiteModelBuilder(AtlesDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<SettingsPageModel> BuildSettingsPageModelAsync(Guid siteId)
        {
            var site = await _dbContext.Sites.FirstOrDefaultAsync(x => x.Id == siteId);

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