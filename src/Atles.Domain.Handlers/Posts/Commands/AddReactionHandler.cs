using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Domain.Models;
using Atles.Domain.Models.Posts;
using Atles.Domain.Models.Posts.Commands;
using Atles.Domain.Models.Posts.Events;
using Atles.Infrastructure.Commands;
using Atles.Infrastructure.Events;
using Microsoft.EntityFrameworkCore;

namespace Atles.Domain.Handlers.Posts.Commands
{
    public class AddReactionHandler : ICommandHandler<AddReaction>
    {
        private readonly AtlesDbContext _dbContext;

        public AddReactionHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<IEvent>> Handle(AddReaction command)
        {
            var post = await _dbContext.Posts
                .Include(x => x.PostReactionSummaries)
                .FirstOrDefaultAsync(x =>
                    x.ForumId == command.ForumId &&
                    x.Forum.Category.SiteId == command.SiteId &&
                    x.Id == command.Id &&
                    x.Status != PostStatusType.Deleted);

            if (post == null)
            {
                throw new DataException($"Post with Id {command.Id} not found.");
            }

            post.AddReactionToSummary(command.Type);

            var postReaction = new PostReaction(command.Id, command.UserId, command.Type);

            _dbContext.PostReactions.Add(postReaction);

            var @event = new ReactionAdded
            {
                Type = command.Type,
                TargetId = command.Id,
                TargetType = nameof(Post),
                SiteId = command.SiteId,
                UserId = command.UserId
            };

            _dbContext.Events.Add(@event.ToDbEntity());

            await _dbContext.SaveChangesAsync();

            return new IEvent[] { @event };
        }
    }
}
