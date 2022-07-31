using System.Data;
using Atles.Commands.Posts;
using Atles.Core.Commands;
using Atles.Core.Events;
using Atles.Core.Results;
using Atles.Core.Results.Types;
using Atles.Data;
using Atles.Domain;
using Atles.Events.Posts;
using Microsoft.EntityFrameworkCore;

namespace Atles.Commands.Handlers.Posts
{
    public class AddPostReactionHandler : ICommandHandler<AddPostReaction>
    {
        private readonly AtlesDbContext _dbContext;

        public AddPostReactionHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CommandResult> Handle(AddPostReaction command)
        {
            var post = await _dbContext.Posts
                .Include(x => x.PostReactionSummaries)
                .FirstOrDefaultAsync(x =>
                    x.ForumId == command.ForumId &&
                    x.Forum.Category.SiteId == command.SiteId &&
                    x.Id == command.PostId &&
                    x.Status != PostStatusType.Deleted);

            if (post == null)
            {
                throw new DataException($"Post with Id {command.PostId} not found.");
            }

            post.AddReactionToSummary(command.Type);

            var postReaction = new PostReaction(command.PostId, command.UserId, command.Type);

            _dbContext.PostReactions.Add(postReaction);

            var @event = new PostReactionAdded
            {
                Type = command.Type,
                TargetId = command.PostId,
                TargetType = nameof(Post),
                SiteId = command.SiteId,
                UserId = command.UserId
            };

            _dbContext.Events.Add(@event.ToDbEntity());

            await _dbContext.SaveChangesAsync();

            return new Success(new IEvent[] { @event });
        }
    }
}
