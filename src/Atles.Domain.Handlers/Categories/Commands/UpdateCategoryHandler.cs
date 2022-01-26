using System.Collections.Generic;
using Atles.Data;
using Atles.Data.Caching;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Threading.Tasks;
using Atles.Domain.Models;
using Atles.Domain.Models.Categories;
using Atles.Domain.Models.Categories.Commands;
using Atles.Domain.Models.Categories.Events;
using Atles.Infrastructure.Commands;
using Atles.Infrastructure.Events;

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

        public async Task<IEnumerable<IEvent>> Handle(UpdateCategory command)
        {
            await _validator.ValidateCommand(command);

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
        }
    }
}
