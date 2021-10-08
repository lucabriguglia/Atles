using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain.Forums;
using Atles.Domain.Forums.Commands;
using Atles.Infrastructure.Commands;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OpenCqrs.Commands;

namespace Atles.Domain.Handlers.Forums.Commands
{
    public class CreateForumHandler : ICommandHandler<CreateForum>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IValidator<CreateForum> _validator;

        public CreateForumHandler(AtlesDbContext dbContext,
            ICacheManager cacheManager,
            IValidator<CreateForum> validator)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
            _validator = validator;
        }

        public async Task Handle(CreateForum command)
        {
            await _validator.ValidateCommandAsync(command);

            var forumsCount = await _dbContext.Forums
                .Where(x => x.CategoryId == command.CategoryId && x.Status != ForumStatusType.Deleted)
                .CountAsync();

            var sortOrder = forumsCount + 1;

            var forum = new Forum(command.Id,
                command.CategoryId,
                command.Name,
                command.Slug,
                command.Description,
                sortOrder,
                command.PermissionSetId);

            _dbContext.Forums.Add(forum);
            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Created,
                typeof(Forum),
                forum.Id,
                new
                {
                    forum.Name,
                    forum.Slug,
                    forum.Description,
                    forum.CategoryId,
                    forum.PermissionSetId,
                    forum.SortOrder
                }));

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Categories(command.SiteId)); 
            _cacheManager.Remove(CacheKeys.CurrentForums(command.SiteId));
        }
    }
}
