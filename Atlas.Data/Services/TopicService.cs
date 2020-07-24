using System.Data;
using System.Threading.Tasks;
using Atlas.Data.Caching;
using Atlas.Domain;
using Atlas.Domain.Topics;
using Atlas.Domain.Topics.Commands;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Data.Services
{
    public class TopicService : ITopicService
    {
        private readonly AtlasDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IValidator<CreateTopic> _createValidator;
        private readonly IValidator<UpdateTopic> _updateValidator;

        public TopicService(AtlasDbContext dbContext,
            ICacheManager cacheManager,
            IValidator<CreateTopic> createValidator,
            IValidator<UpdateTopic> updateValidator)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task CreateAsync(CreateTopic command)
        {
            await _createValidator.ValidateCommandAsync(command);

            var topic = new Topic(command.Id,
                command.ForumId,
                command.MemberId,
                command.Title,
                command.Content,
                command.Status);

            _dbContext.Topics.Add(topic);
            _dbContext.Events.Add(new Event(command.SiteId,
                command.MemberId,
                EventType.Created,
                typeof(Topic),
                command.Id,
                new 
                {
                    command.ForumId,
                    command.Title,
                    command.Content,
                    command.Status
                }));

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(UpdateTopic command)
        {
            await _updateValidator.ValidateCommandAsync(command);

            var topic = await _dbContext.Topics
                .FirstOrDefaultAsync(x =>
                    x.Id == command.Id &&
                    x.Status != StatusType.Deleted);

            if (topic == null)
            {
                throw new DataException($"Topic with Id {command.Id} not found.");
            }

            topic.UpdateDetails(command.Title, command.Content, command.Status);
            _dbContext.Events.Add(new Event(command.SiteId,
                command.MemberId,
                EventType.Updated,
                typeof(Topic),
                command.Id,
                new
                {
                    command.Title,
                    command.Content,
                    command.Status
                }));

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(DeleteTopic command)
        {
            var topic = await _dbContext.Topics
                .Include(x => x.Replies)
                .FirstOrDefaultAsync(x =>
                    x.Id == command.Id &&
                    x.Status != StatusType.Deleted);

            if (topic == null)
            {
                throw new DataException($"Topic with Id {command.Id} not found.");
            }

            topic.Delete();
            _dbContext.Events.Add(new Event(command.SiteId,
                command.MemberId,
                EventType.Deleted,
                typeof(Topic),
                command.Id));

            foreach (var reply in topic.Replies)
            {
                reply.Delete();
                _dbContext.Events.Add(new Event(command.SiteId,
                    command.MemberId,
                    EventType.Deleted,
                    typeof(Reply),
                    reply.Id));
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}