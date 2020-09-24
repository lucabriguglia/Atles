using System.Data;
using System.Threading.Tasks;
using Atles.Data.Caching;
using Atles.Domain;
using Atles.Domain.Themes;
using Atles.Domain.Themes.Commands;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Atles.Data.Services
{
    /// <inheritdoc />
    public class ThemeService : IThemeService
    {
        private readonly AtlesDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IValidator<CreateTheme> _createValidator;
        private readonly IValidator<UpdateTheme> _updateValidator;

        /// <summary>
        /// ThemeService
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="cacheManager"></param>
        /// <param name="createValidator"></param>
        /// <param name="updateValidator"></param>
        public ThemeService(AtlesDbContext dbContext,
            ICacheManager cacheManager,
            IValidator<CreateTheme> createValidator,
            IValidator<UpdateTheme> updateValidator)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <inheritdoc />
        public async Task CreateAsync(CreateTheme command)
        {
            await _createValidator.ValidateCommandAsync(command);

            var entity = Theme.CreateNew(command.Id,
                command.SiteId,
                command.Name);

            _dbContext.Themes.Add(entity);

            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Created,
                typeof(Theme),
                entity.Id,
                new
                {
                    entity.Name
                }));

            await _dbContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task UpdateAsync(UpdateTheme command)
        {
            await _updateValidator.ValidateCommandAsync(command);

            var entity = await _dbContext.Themes
                .FirstOrDefaultAsync(x =>
                    x.SiteId == command.SiteId &&
                    x.Id == command.Id &&
                    x.Status != ThemeStatus.Deleted);

            if (entity == null)
            {
                throw new DataException($"Theme with Id {command.Id} not found.");
            }

            entity.UpdateDetails(command.Name);

            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Updated,
                typeof(Theme),
                entity.Id,
                new
                {
                    entity.Name
                }));

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Theme(command.Id));
        }

        /// <inheritdoc />
        public async Task DeleteAsync(DeleteTheme command)
        {
            var entity = await _dbContext.Themes
                .FirstOrDefaultAsync(x =>
                    x.SiteId == command.SiteId &&
                    x.Id == command.Id &&
                    x.Status != ThemeStatus.Deleted);

            if (entity == null)
            {
                throw new DataException($"Theme with Id {command.Id} not found.");
            }

            entity.Delete();

            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Deleted,
                typeof(Theme),
                entity.Id));

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Theme(command.Id));
        }

        /// <inheritdoc />
        public async Task RestoreAsync(RestoreTheme command)
        {
            var entity = await _dbContext.Themes
                .FirstOrDefaultAsync(x =>
                    x.SiteId == command.SiteId &&
                    x.Id == command.Id &&
                    x.Status == ThemeStatus.Deleted);

            if (entity == null)
            {
                throw new DataException($"Theme with Id {command.Id} not found.");
            }

            entity.Restore();

            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Restored,
                typeof(Theme),
                entity.Id));

            await _dbContext.SaveChangesAsync();
        }
    }
}