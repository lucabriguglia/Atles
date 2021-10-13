using Atles.Data;
using Atles.Domain.Posts;
using Atles.Models;
using Atles.Models.Public.Forums;
using Atles.Reporting.Handlers.Services;
using Atles.Reporting.Public.Queries;
using Microsoft.EntityFrameworkCore;
using OpenCqrs;
using OpenCqrs.Queries;
using System.Linq;
using System.Threading.Tasks;

namespace Atles.Reporting.Handlers.Public
{
    public class GetForumPageTopicsHandler : IQueryHandler<GetForumPageTopics, PaginatedData<ForumPageModel.TopicModel>>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly ISender _sender;
        private readonly IGravatarService _gravatarService;
        public GetForumPageTopicsHandler(AtlesDbContext dbContext, ISender sender, IGravatarService gravatarService)
        {
            _dbContext = dbContext;
            _sender = sender;
            _gravatarService = gravatarService;
        }

        public async Task<PaginatedData<ForumPageModel.TopicModel>> Handle(GetForumPageTopics query)
        {
            var topicsQuery = _dbContext.Posts
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
                GravatarHash = _gravatarService.GenerateEmailHash(topic.CreatedByUser.Email),
                MostRecentUserId = topic.LastReply?.CreatedBy ?? topic.CreatedBy,
                MostRecentUserDisplayName = topic.LastReply?.CreatedByUser?.DisplayName ?? topic.CreatedByUser.DisplayName,
                MostRecentTimeStamp = topic.LastReply?.CreatedOn ?? topic.CreatedOn,
                Pinned = topic.Pinned,
                Locked = topic.Locked,
                HasAnswer = topic.HasAnswer
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
}
