using System.Data;
using System.Threading.Tasks;
using Atlify.Data.Caching;
using Atlify.Domain;
using Atlify.Domain.Sites;
using Atlify.Domain.Sites.Commands;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Atlify.Data.Services
{
    public class SiteService : ISiteService
    {
        private readonly AtlifyDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IValidator<UpdateSite> _updateValidator;

        public SiteService(AtlifyDbContext dbContext,
            ICacheManager cacheManager,
            IValidator<UpdateSite> updateValidator)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
            _updateValidator = updateValidator;
        }

        public async Task UpdateAsync(UpdateSite command)
        {
            await _updateValidator.ValidateCommandAsync(command);

            var site = await _dbContext.Sites
                .FirstOrDefaultAsync(x => 
                    x.Id == command.SiteId);

            if (site == null)
            {
                throw new DataException($"Site with Id {command.SiteId} not found.");
            }

            site.UpdateDetails(command.Title, command.Theme, command.Css, command.Language, command.Privacy, command.Terms);

            _dbContext.Events.Add(new Event(site.Id,
                command.MemberId,
                EventType.Updated,
                typeof(Site),
                site.Id,
                new
                {
                    site.Title,
                    site.PublicTheme,
                    site.PublicCss,
                    site.Language,
                    site.Privacy,
                    site.Terms
                }));

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.CurrentSite(site.Name));
        }
    }
}
