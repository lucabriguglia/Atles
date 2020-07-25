using System;
using System.Threading.Tasks;
using Atlas.Models.Admin.Site;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Data.Builders.Admin
{
    public class SiteModelBuilder : ISiteModelBuilder
    {
        private readonly AtlasDbContext _dbContext;

        public SiteModelBuilder(AtlasDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SettingsPageModel> BuildSettingsPageModelAsync(Guid siteId)
        {
            var site = await _dbContext.Sites.FirstOrDefaultAsync(x => x.Id == siteId);

            if (site == null)
            {
                return null;
            }

            return new SettingsPageModel
            {
                SiteId = site.Id,
                Title = site.Title
            };
        }
    }
}