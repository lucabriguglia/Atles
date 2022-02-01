using Atles.Core.Commands;
using Atles.Core.Events;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain.Commands.Handlers.Extensions;
using Atles.Domain.Events;
using Atles.Domain.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Atles.Domain.Commands.Handlers
{
    public class CreateCategoryHandler : ICommandHandler<CreateCategory>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly IValidator<CreateCategory> _validator;
        private readonly ICacheManager _cacheManager;

        public CreateCategoryHandler(AtlesDbContext dbContext, IValidator<CreateCategory> validator, ICacheManager cacheManager)
        {
            _dbContext = dbContext;
            _validator = validator;
            _cacheManager = cacheManager;
        }

        public async Task<IEnumerable<IEvent>> Handle(CreateCategory command)
        {
            await _validator.ValidateCommand(command);

            var categoriesCount = await _dbContext.Categories
                .Where(x => x.SiteId == command.SiteId && x.Status != CategoryStatusType.Deleted)
                .CountAsync();

            var sortOrder = categoriesCount + 1;

            var category = new Category(command.CategoryId, command.SiteId, command.Name, sortOrder, command.PermissionSetId);

            _dbContext.Categories.Add(category);

            var @event = new CategoryCreated
            {
                Name = category.Name,
                PermissionSetId = category.PermissionSetId,
                SortOrder = category.SortOrder,
                TargetId = category.Id,
                TargetType = nameof(Category),
                SiteId = command.SiteId,
                UserId = command.UserId
            };

            _dbContext.Events.Add(@event.ToDbEntity());

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Categories(command.SiteId));
            _cacheManager.Remove(CacheKeys.CurrentForums(command.SiteId));

            return new IEvent[] { @event };
        }
    }
}
