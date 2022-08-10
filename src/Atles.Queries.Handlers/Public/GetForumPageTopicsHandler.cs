using Atles.Core.Extensions;
using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Data;
using Atles.Domain;
using Atles.Models;
using Atles.Models.Public;
using Atles.Queries.Public;
using Microsoft.EntityFrameworkCore;

namespace Atles.Queries.Handlers.Public;

public class GetForumPageTopicsHandler : IQueryHandler<GetForumPageTopics, PaginatedData<ForumPageModel.TopicModel>>
{
    private readonly AtlesDbContext _dbContext;

    public GetForumPageTopicsHandler(AtlesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<QueryResult<PaginatedData<ForumPageModel.TopicModel>>> Handle(GetForumPageTopics query)
    {
        var topicsQuery = _dbContext.Posts
            .Include(x => x.PostReactionSummaries)
            .Include(x => x.CreatedByUser)
            .Include(x => x.LastReply).ThenInclude(x => x.CreatedByUser)
            .Where(x =>
                x.TopicId == null &&
                x.ForumId == query.ForumId &&
                x.Status == PostStatusType.Published);

        if (!string.IsNullOrWhiteSpace(query.Options.Search))
        {
            topicsQuery = topicsQuery
                .Where(x => x.Title.Contains(query.Options.Search) || x.Content.Contains(query.Options.Search));
        }

        var topics = await topicsQuery
            .OrderByDescending(x => x.Pinned)
            .ThenByDescending(x => x.LastReply != null ? x.LastReply.CreatedOn : x.CreatedOn)
            .Skip(query.Options.Skip)
            .Take(query.Options.PageSize)
            .ToListAsync();

        var items = topics.Select(topic => new ForumPageModel.TopicModel
            {
                Id = topic.Id,
                Title = topic.Title,
                Slug = topic.Slug,
                TotalReplies = topic.RepliesCount,
                UserId = topic.CreatedByUser.Id,
                UserDisplayName = topic.CreatedByUser.DisplayName,
                TimeStamp = topic.CreatedOn,
                GravatarHash = topic.CreatedByUser.Email.ToGravatarEmailHash(),
                MostRecentUserId = topic.LastReply?.CreatedBy ?? topic.CreatedBy,
                MostRecentUserDisplayName = topic.LastReply?.CreatedByUser?.DisplayName ?? topic.CreatedByUser.DisplayName,
                MostRecentTimeStamp = topic.LastReply?.CreatedOn ?? topic.CreatedOn,
                Pinned = topic.Pinned,
                Locked = topic.Locked,
                HasAnswer = topic.HasAnswer,
                Reactions = topic.PostReactionSummaries.Select(x => new ForumPageModel.ReactionModel { Type = x.Type, Count = x.Count }).ToList()
            })
            .ToList();

        var totalRecordsQuery = _dbContext.Posts
            .Where(x =>
                x.TopicId == null &&
                x.ForumId == query.ForumId &&
                x.Status == PostStatusType.Published);

        if (!string.IsNullOrWhiteSpace(query.Options.Search))
        {
            totalRecordsQuery = totalRecordsQuery
                .Where(x => x.Title.Contains(query.Options.Search) || x.Content.Contains(query.Options.Search));
        }

        var totalRecords = await totalRecordsQuery.CountAsync();

        var result = new PaginatedData<ForumPageModel.TopicModel>(items, totalRecords, query.Options.PageSize);

        return result;
    }
}