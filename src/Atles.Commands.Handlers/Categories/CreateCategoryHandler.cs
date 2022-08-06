using Atles.Commands.Categories;
using Atles.Core.Commands;
using Atles.Core.Events;
using Atles.Core.Results;
using Atles.Core.Results.Types;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain;
using Atles.Events.Categories;
using Microsoft.EntityFrameworkCore;

namespace Atles.Commands.Handlers.Categories;

public class CreateCategoryHandler : ICommandHandler<CreateCategory>
{
    private readonly AtlesDbContext _dbContext;
    private readonly ICacheManager _cacheManager;

    public CreateCategoryHandler(AtlesDbContext dbContext, ICacheManager cacheManager)
    {
        _dbContext = dbContext;
        _cacheManager = cacheManager;
    }

    public async Task<CommandResult> Handle(CreateCategory command)
    {
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

        return new Success(new IEvent[] { @event });
    }
}
