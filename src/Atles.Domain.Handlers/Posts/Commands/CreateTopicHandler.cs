using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain.Posts;
using Atles.Domain.Posts.Commands;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OpenCqrs.Commands;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Atles.Domain.Handlers.Posts.Commands
{
    public class CreateTopicHandler : ICommandHandler<CreateTopic>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly IValidator<CreateTopic> _validator;
        private readonly ICacheManager _cacheManager;

        public CreateTopicHandler(AtlesDbContext dbContext, IValidator<CreateTopic> validator, ICacheManager cacheManager)
        {
            _dbContext = dbContext;
            _validator = validator;
            _cacheManager = cacheManager;
        }

        public async Task Handle(CreateTopic command)
        {
            await _validator.ValidateCommandAsync(command);

            var title = Regex.Replace(command.Title, @"\s+", " "); // Remove multiple spaces from title

            var topic = Post.CreateTopic(command.Id,
                command.ForumId,
                command.UserId,
                title,
                command.Slug,
                command.Content,
                command.Status);

            _dbContext.Posts.Add(topic);
            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Created,
                typeof(Post),
                command.Id,
                new
                {
                    command.ForumId,
                    title,
                    command.Slug,
                    command.Content,
                    command.Status
                }));

            var forum = await _dbContext.Forums.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == topic.ForumId);
            forum.UpdateLastPost(topic.Id);
            forum.IncreaseTopicsCount();
            forum.Category.IncreaseTopicsCount();

            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == topic.CreatedBy);
            user.IncreaseTopicsCount();

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.Forum(forum.Id));
        }
    }
}
