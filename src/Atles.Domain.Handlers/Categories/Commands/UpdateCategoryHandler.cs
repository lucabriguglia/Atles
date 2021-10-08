using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain.Categories;
using Atles.Domain.Categories.Commands;
using Atles.Infrastructure.Commands;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OpenCqrs.Commands;
using System.Data;
using System.Threading.Tasks;

namespace Atles.Domain.Handlers.Categories.Commands
{
    public class UpdateCategoryHandler : ICommandHandler<UpdateCategory>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly IValidator<UpdateCategory> _validator;
        private readonly ICacheManager _cacheManager;

        public UpdateCategoryHandler(AtlesDbContext dbContext, IValidator<UpdateCategory> validator, ICacheManager cacheManager)
        {
            _dbContext = dbContext;
            _validator = validator;
            _cacheManager = cacheManager;
        }

        public async Task Handle(UpdateCategory command)
        {
            await _validator.ValidateCommandAsync(command);

            var category = await _dbContext.Categories
                .FirstOrDefaultAsync(x =>
                    x.SiteId == command.SiteId &&
                    x.Id == command.Id &&
                    x.Status != CategoryStatusType.Deleted);

            if (category == null)
            {
                throw new DataException($"Category with Id {command.Id} not found.");
            }

            category.UpdateDetails(command.Name, command.PermissionSetId);

            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Updated,
                typeof(Category),
                category.Id,
                new
                {
                    category.Name,
                    category.PermissionSetId
                }));

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Categories(command.SiteId));
            _cacheManager.Remove(CacheKeys.CurrentForums(command.SiteId));
        }
    }
}
