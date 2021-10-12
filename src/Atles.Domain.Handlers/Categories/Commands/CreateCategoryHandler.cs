using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain.Categories;
using Atles.Domain.Categories.Commands;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OpenCqrs.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace Atles.Domain.Handlers.Categories.Commands
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

        public async Task Handle(CreateCategory command)
        {
            await _validator.ValidateCommandAsync(command);

            var categoriesCount = await _dbContext.Categories
                .Where(x => x.SiteId == command.SiteId && x.Status != CategoryStatusType.Deleted)
                .CountAsync();

            var sortOrder = categoriesCount + 1;

            var category = new Category(command.Id,
                                        command.SiteId,
                                        command.Name,
                                        sortOrder,
                                        command.PermissionSetId);

            _dbContext.Categories.Add(category);

            Event evnt = new(EventType.Created,
                            category.Id,
                            typeof(Category),
                            command.SiteId,
                            command.UserId,
                            new
                            {
                                category.Name,
                                category.PermissionSetId,
                                category.SortOrder
                            });

            _dbContext.Events.Add(evnt);

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Categories(command.SiteId));
            _cacheManager.Remove(CacheKeys.CurrentForums(command.SiteId));
        }
    }
}
