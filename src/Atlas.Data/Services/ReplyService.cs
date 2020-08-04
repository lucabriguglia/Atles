using System.Data;
using System.Threading.Tasks;
using Atlas.Data.Caching;
using Atlas.Domain;
using Atlas.Domain.Replies;
using Atlas.Domain.Replies.Commands;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Data.Services
{
    public class ReplyService : IReplyService
    {
        private readonly AtlasDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IValidator<CreateReply> _createValidator;
        private readonly IValidator<UpdateReply> _updateValidator;

        public ReplyService(AtlasDbContext dbContext,
            ICacheManager cacheManager,
            IValidator<CreateReply> createValidator,
            IValidator<UpdateReply> updateValidator)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task CreateAsync(CreateReply command)
        {
            await _createValidator.ValidateCommandAsync(command);

            var reply = new Reply(command.Id,
                command.TopicId,
                command.MemberId,
                command.Content,
                command.Status);

            _dbContext.Replies.Add(reply);

            _dbContext.Events.Add(new Event(command.SiteId,
                command.MemberId,
                EventType.Created,
                typeof(Reply),
                command.Id,
                new 
                {
                    command.TopicId,
                    command.Content,
                    command.Status
                }));

            var topic = await _dbContext.Topics
                    .Include(x => x.Forum)
                        .ThenInclude(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == reply.TopicId);

            topic.IncreaseRepliesCount();
            topic.Forum.IncreaseRepliesCount();
            topic.Forum.Category.IncreaseRepliesCount();

            var member = await _dbContext.Members.FirstOrDefaultAsync(x => x.Id == reply.MemberId);
            member.IncreaseRepliesCount();

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(UpdateReply command)
        {
            await _updateValidator.ValidateCommandAsync(command);

            var reply = await _dbContext.Replies
                .FirstOrDefaultAsync(x =>
                    x.Id == command.Id &&
                    x.TopicId == command.TopicId &&
                    x.Topic.ForumId == command.ForumId &&
                    x.Topic.Forum.Category.SiteId == command.SiteId &&
                    x.Status != StatusType.Deleted);

            if (reply == null)
            {
                throw new DataException($"Reply with Id {command.Id} not found.");
            }

            reply.UpdateDetails(command.Content, command.Status);

            _dbContext.Events.Add(new Event(command.SiteId,
                command.MemberId,
                EventType.Updated,
                typeof(Reply),
                command.Id,
                new
                {
                    command.Content,
                    command.Status
                }));

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(DeleteReply command)
        {
            var reply = await _dbContext.Replies
                .Include(x => x.Member)
                .Include(x => x.Topic)
                    .ThenInclude(x => x.Forum)
                        .ThenInclude(x => x.Category)
                .FirstOrDefaultAsync(x =>
                    x.Id == command.Id &&
                    x.TopicId == command.TopicId &&
                    x.Topic.ForumId == command.ForumId &&
                    x.Topic.Forum.Category.SiteId == command.SiteId &&
                    x.Status != StatusType.Deleted);

            if (reply == null)
            {
                throw new DataException($"Reply with Id {command.Id} not found.");
            }

            reply.Delete();

            _dbContext.Events.Add(new Event(command.SiteId,
                command.MemberId,
                EventType.Deleted,
                typeof(Reply),
                command.Id));

            reply.Topic.DecreaseRepliesCount();
            reply.Topic.Forum.DecreaseRepliesCount();
            reply.Topic.Forum.Category.DecreaseRepliesCount();
            reply.Member.DecreaseRepliesCount();

            await _dbContext.SaveChangesAsync();
        }
    }
}