using System.Data;
using Atles.Commands.Categories;
using Atles.Core.Commands;
using Atles.Core.Events;
using Atles.Core.Results;
using Atles.Core.Results.Types;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain;
using Atles.Events.Categories;
using Atles.Models.Admin;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Atles.Commands.Handlers.Categories
{
    public class UpdateCategoryHandler : ICommandHandler<UpdateCategory>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly ICacheManager _cacheManager;

        public UpdateCategoryHandler(AtlesDbContext dbContext, ICacheManager cacheManager)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
        }

        public async Task<CommandResult> Handle(UpdateCategory command)
        {
            var category = await _dbContext.Categories
                .FirstOrDefaultAsync(x =>
                    x.SiteId == command.SiteId &&
                    x.Id == command.CategoryId &&
                    x.Status != CategoryStatusType.Deleted);

            if (category == null)
            {
                throw new DataException($"Category with Id {command.CategoryId} not found.");
            }

            category.UpdateDetails(command.Name, command.PermissionSetId);

            var @event = new CategoryUpdated
            {
                Name = category.Name,
                PermissionSetId = category.PermissionSetId,
                TargetId = category.Id,
                TargetType = nameof(Category),
                SiteId = command.SiteId,
                UserId = command.UserId
            };

            _dbContext.Events.Add(@event.ToDbEntity());

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Categories(command.SiteId));
            _cacheManager.Remove(CacheKeys.CurrentForums(command.SiteId));

            return new Success(new IEvent[] { @event });
        }
    }
}
