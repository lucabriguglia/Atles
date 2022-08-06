using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Core.Results.Types;
using Atles.Data;
using Atles.Models.Admin.Sites;
using Atles.Queries.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Atles.Queries.Handlers.Admin
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

        public async Task<QueryResult<SettingsPageModel>> Handle(GetSettingsPageModel query)
        {
            var site = await _dbContext.Sites.FirstOrDefaultAsync(x => x.Id == query.SiteId);

            if (site == null)
            {
                return new Failure(FailureType.NotFound, "Site", $"Site with id {query.SiteId} not found.");
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
                    SiteId = site.Id,
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
