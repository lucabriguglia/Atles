using Atles.Data;
using Atles.Data.Caching;
using Markdig;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Atles.Infrastructure.Queries;
using Atles.Reporting.Models.Public;
using Atles.Reporting.Models.Public.Queries;

namespace Atles.Reporting.Handlers.Public
{
    public class GetCurrentSiteHandler : IQueryHandler<GetCurrentSite, CurrentSiteModel>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly ICacheManager _cacheManager;

        public GetCurrentSiteHandler(AtlesDbContext dbContext, ICacheManager cacheManager)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
        }

        public async Task<CurrentSiteModel> Handle(GetCurrentSite query)
        {
            var currentSite = await _cacheManager.GetOrSetAsync(CacheKeys.CurrentSite("Default"), () =>
                _dbContext.Sites.FirstOrDefaultAsync(x => x.Name == "Default"));

            return new CurrentSiteModel
            {
                Id = currentSite.Id,
                Name = currentSite.Name,
                Title = currentSite.Title,
                Theme = currentSite.PublicTheme,
                CssPublic = currentSite.PublicCss,
                CssAdmin = currentSite.AdminCss,
                Language = currentSite.Language,
                Privacy = Markdown.ToHtml(currentSite.Privacy),
                Terms = Markdown.ToHtml(currentSite.Terms),
                HeadScript = currentSite.HeadScript
            };
        }
    }
}
