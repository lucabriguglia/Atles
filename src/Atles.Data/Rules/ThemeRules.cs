using System;
using System.Threading.Tasks;
using Atles.Domain.Themes;
using Microsoft.EntityFrameworkCore;

namespace Atles.Data.Rules
{
    /// <inheritdoc />
    public class ThemeRules : IThemeRules
    {
        private readonly AtlesDbContext _dbContext;

        /// <summary>
        /// ThemeRules
        /// </summary>
        /// <param name="dbContext"></param>
        public ThemeRules(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task<bool> IsNameUniqueAsync(Guid siteId, string name)
        {
            var any = await _dbContext.Themes
                .AnyAsync(x => x.SiteId == siteId &&
                               x.Name == name &&
                               x.Status != ThemeStatus.Deleted);
            return !any;
        }

        /// <inheritdoc />
        public async Task<bool> IsNameUniqueAsync(Guid siteId, string name, Guid id)
        {
            var any = await _dbContext.Themes
                .AnyAsync(x => x.SiteId == siteId &&
                               x.Name == name &&
                               x.Status != ThemeStatus.Deleted &&
                               x.Id != id);
            return !any;
        }
    }
}