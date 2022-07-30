using Atles.Data;
using Atles.Reporting.Handlers.Services;
using Markdig;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Atles.Core.Queries;
using Atles.Domain;
using Atles.Reporting.Models.Public;
using Atles.Reporting.Models.Public.Queries;
using Atles.Reporting.Models.Shared;

namespace Atles.Reporting.Handlers.Public
{
    public class GetTopicPageRepliesHandler : IQueryHandler<GetTopicPageReplies, PaginatedData<TopicPageModel.ReplyModel>>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly IGravatarService _gravatarService;

        public GetTopicPageRepliesHandler(AtlesDbContext dbContext, IGravatarService gravatarService)
        {
            _dbContext = dbContext;
            _gravatarService = gravatarService;
        }

        public async Task<PaginatedData<TopicPageModel.ReplyModel>> Handle(GetTopicPageReplies query)
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
                GravatarHash = _gravatarService.GenerateEmailHash(reply.CreatedByUser.Email),
                IsAnswer = reply.IsAnswer,
                Reactions = reply.PostReactionSummaries.Select(x => new TopicPageModel.ReactionModel { Type = x.Type, Count = x.Count }).ToList()
            }).ToList();

            var totalRecords = await repliesQuery.CountAsync();

            var result = new PaginatedData<TopicPageModel.ReplyModel>(items, totalRecords, query.Options.PageSize);

            return result;
        }
    }
}
