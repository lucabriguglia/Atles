using Atles.Data;
using Atles.Domain.Posts;
using Atles.Models.Public.Topics;
using Atles.Reporting.Public.Queries;
using Atles.Reporting.Shared.Queries;
using Markdig;
using Microsoft.EntityFrameworkCore;
using OpenCqrs;
using OpenCqrs.Queries;
using System.Linq;
using System.Threading.Tasks;

namespace Atles.Reporting.Handlers.Public
{
    public class GetTopicPageHandler : IQueryHandler<GetTopicPage, TopicPageModel>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly ISender _sender;

        public GetTopicPageHandler(AtlesDbContext dbContext, ISender sender)
        {
            _dbContext = dbContext;
            _sender = sender;
        }

        public async Task<TopicPageModel> Handle(GetTopicPage query)
        {
            var topic = await _dbContext.Posts
                .Include(x => x.Forum).ThenInclude(x => x.Category)
                .Include(x => x.CreatedByUser)
                .FirstOrDefaultAsync(x =>
                    x.TopicId == null &&
                    x.Forum.Category.SiteId == query.SiteId &&
                    x.Forum.Slug == query.ForumSlug &&
                    x.Slug == query.TopicSlug &&
                    x.Status == PostStatusType.Published);

            if (topic == null)
            {
                return null;
            }

            var result = new TopicPageModel
            {
                Forum = new TopicPageModel.ForumModel
                {
                    Id = topic.Forum.Id,
                    Name = topic.Forum.Name,
                    Slug = topic.Forum.Slug
                },
                Topic = new TopicPageModel.TopicModel
                {
                    Id = topic.Id,
                    Title = topic.Title,
                    Slug = topic.Slug,
                    Content = Markdown.ToHtml(topic.Content),
                    UserId = topic.CreatedByUser.Id,
                    UserDisplayName = topic.CreatedByUser.DisplayName,
                    TimeStamp = topic.CreatedOn,
                    IdentityUserId = topic.CreatedByUser.IdentityUserId,
                    GravatarHash = _sender.Send(new GenerateEmailHashForGravatar { Email = topic.CreatedByUser.Email }).GetAwaiter().GetResult(),
                    Pinned = topic.Pinned,
                    Locked = topic.Locked,
                    HasAnswer = topic.HasAnswer
                },
                Replies = await _sender.Send(new GetTopicPageReplies { TopicId = topic.Id, Options = query.Options })
            };

            if (topic.HasAnswer)
            {
                var answer = await _dbContext.Posts
                    .Include(x => x.CreatedByUser)
                    .Where(x =>
                        x.TopicId == topic.Id &&
                        x.Status == PostStatusType.Published &&
                        x.IsAnswer)
                    .FirstOrDefaultAsync();

                if (answer != null)
                {
                    result.Answer = new TopicPageModel.ReplyModel
                    {
                        Id = answer.Id,
                        Content = Markdown.ToHtml(answer.Content),
                        OriginalContent = answer.Content,
                        IdentityUserId = answer.CreatedByUser.IdentityUserId,
                        UserId = answer.CreatedByUser.Id,
                        UserDisplayName = answer.CreatedByUser.DisplayName,
                        TimeStamp = answer.CreatedOn,
                        GravatarHash = _sender.Send(new GenerateEmailHashForGravatar { Email = answer.CreatedByUser.Email }).GetAwaiter().GetResult(),
                        IsAnswer = answer.IsAnswer
                    };
                }
            }

            return result;
        }
    }
}
