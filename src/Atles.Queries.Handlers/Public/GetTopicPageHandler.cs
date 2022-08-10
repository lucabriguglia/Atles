using Atles.Core;
using Atles.Core.Extensions;
using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Data;
using Atles.Domain;
using Atles.Models.Public;
using Atles.Queries.Public;
using Markdig;
using Microsoft.EntityFrameworkCore;

namespace Atles.Queries.Handlers.Public;

public class GetTopicPageHandler : IQueryHandler<GetTopicPage, TopicPageModel>
{
    private readonly AtlesDbContext _dbContext;
    private readonly IDispatcher _dispatcher;
    public GetTopicPageHandler(AtlesDbContext dbContext, IDispatcher sender)
    {
        _dbContext = dbContext;
        _dispatcher = sender;
    }

    public async Task<QueryResult<TopicPageModel>> Handle(GetTopicPage query)
    {
        var topic = await _dbContext.Posts
            .Include(x => x.PostReactionSummaries)
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

        var subscription = await _dbContext.Subscriptions.FirstOrDefaultAsync(x => x.UserId == query.UserId && x.ItemId == topic.Id);

        // TODO: To be moved to a service
        var queryResult = await _dispatcher.Get(new GetTopicPageReplies { TopicId = topic.Id, Options = query.Options });
        var replies = queryResult.AsT0;

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
                GravatarHash = topic.CreatedByUser.Email.ToGravatarEmailHash(),
                Pinned = topic.Pinned,
                Locked = topic.Locked,
                HasAnswer = topic.HasAnswer,
                Subscribed = subscription != null,
                Reactions = topic.PostReactionSummaries.Select(x => new TopicPageModel.ReactionModel { Type = x.Type, Count = x.Count }).ToList()
            },
            Replies = replies
        };

        if (topic.HasAnswer)
        {
            var answer = await _dbContext.Posts
                .Include(x => x.PostReactionSummaries)
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
                    GravatarHash = answer.CreatedByUser.Email.ToGravatarEmailHash(),
                    IsAnswer = answer.IsAnswer,
                    Reactions = answer.PostReactionSummaries.Select(x => new TopicPageModel.ReactionModel { Type = x.Type, Count = x.Count }).ToList()
                };
            }
        }

        return result;
    }
}