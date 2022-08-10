using Atles.Core.Extensions;
using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Data;
using Atles.Domain;
using Atles.Models;
using Atles.Models.Public;
using Atles.Queries.Public;
using Markdig;
using Microsoft.EntityFrameworkCore;

namespace Atles.Queries.Handlers.Public;

public class GetTopicPageRepliesHandler : IQueryHandler<GetTopicPageReplies, PaginatedData<TopicPageModel.ReplyModel>>
{
    private readonly AtlesDbContext _dbContext;

    public GetTopicPageRepliesHandler(AtlesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<QueryResult<PaginatedData<TopicPageModel.ReplyModel>>> Handle(GetTopicPageReplies query)
    {
        var repliesQuery = _dbContext.Posts
            .Include(x => x.PostReactionSummaries)
            .Include(x => x.CreatedByUser)
            .Where(x =>
                x.TopicId == query.TopicId &&
                x.Status == PostStatusType.Published &&
                x.IsAnswer == false);

        if (!string.IsNullOrWhiteSpace(query.Options.Search))
        {
            repliesQuery = repliesQuery.Where(x => x.Content.Contains(query.Options.Search));
        }

        var replies = await repliesQuery
            .OrderBy(x => x.CreatedOn)
            .Skip(query.Options.Skip)
            .Take(query.Options.PageSize)
            .ToListAsync();

        var items = replies.Select(reply => new TopicPageModel.ReplyModel
        {
            Id = reply.Id,
            Content = Markdown.ToHtml(reply.Content),
            OriginalContent = reply.Content,
            IdentityUserId = reply.CreatedByUser.IdentityUserId,
            UserId = reply.CreatedByUser.Id,
            UserDisplayName = reply.CreatedByUser.DisplayName,
            TimeStamp = reply.CreatedOn,
            GravatarHash = reply.CreatedByUser.Email.ToGravatarEmailHash(),
            IsAnswer = reply.IsAnswer,
            Reactions = reply.PostReactionSummaries.Select(x => new TopicPageModel.ReactionModel { Type = x.Type, Count = x.Count }).ToList()
        }).ToList();

        var totalRecords = await repliesQuery.CountAsync();

        var result = new PaginatedData<TopicPageModel.ReplyModel>(items, totalRecords, query.Options.PageSize);

        return result;
    }
}
