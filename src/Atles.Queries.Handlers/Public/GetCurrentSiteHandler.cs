using System.Threading.Tasks;
using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Models.Public;
using Atles.Queries.Public;
using Markdig;
using Microsoft.EntityFrameworkCore;

namespace Atles.Queries.Handlers.Public
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

        public async Task<QueryResult<CurrentSiteModel>> Handle(GetCurrentSite query)
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
