using System;
using System.Linq;
using System.Threading.Tasks;
using Atles.Domain.Themes;
using Microsoft.EntityFrameworkCore;

namespace Atles.Data.Builders.Admin
{
    /// <inheritdoc />
    public class ThemeModelBuilder : IThemeModelBuilder
    {
        private readonly AtlesDbContext _dbContext;

        /// <summary>
        /// ThemeModelBuilder
        /// </summary>
        /// <param name="dbContext"></param>
        public ThemeModelBuilder(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task<IndexPageModel> BuildIndexPageModelAsync(Guid siteId)
        {
            var result = new IndexPageModel();

            var entities = await _dbContext.Themes
                .Where(x => x.SiteId == siteId && x.Status != ThemeStatus.Deleted)
                .OrderBy(x => x.Name)
                .ToListAsync();

            foreach (var entity in entities)
            {
                result.Themes.Add(new IndexPageModel.ThemeModel
                {
                    Id = entity.Id,
                    Name = entity.Name
                });
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<FormComponentModel> BuildFormModelAsync(Guid siteId, Guid? id = null)
        {
            var result = new FormComponentModel();

            if (id != null)
            {
                var entity = await _dbContext.Themes
                    .FirstOrDefaultAsync(x =>
                        x.SiteId == siteId &&
                        x.Id == id &&
                        x.Status != ThemeStatus.Deleted);

                if (entity == null)
                {
                    return null;
                }

                result.Theme = new FormComponentModel.ThemeModel
                {
                    Id = entity.Id,
                    Name = entity.Name
                };
            }
            else
            {
                result.Theme = new FormComponentModel.ThemeModel();
            }

            return result;
        }
    }
}