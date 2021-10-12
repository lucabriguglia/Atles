using Atles.Data;
using Atles.Domain.Posts;
using Atles.Models;
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
    public class GetTopicPageRepliesHandler : IQueryHandler<GetTopicPageReplies, PaginatedData<TopicPageModel.ReplyModel>>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly ISender _sender;

        public GetTopicPageRepliesHandler(AtlesDbContext dbContext, ISender sender)
        {
            _dbContext = dbContext;
            _sender = sender;
        }

        public async Task<PaginatedData<TopicPageModel.ReplyModel>> Handle(GetTopicPageReplies query)
        {
            var repliesQuery = _dbContext.Posts
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
                GravatarHash = _sender.Send(new GenerateEmailHashForGravatar { Email = reply.CreatedByUser.Email }).GetAwaiter().GetResult(),
                IsAnswer = reply.IsAnswer
            }).ToList();

            var totalRecords = await repliesQuery.CountAsync();

            var result = new PaginatedData<TopicPageModel.ReplyModel>(items, totalRecords, query.Options.PageSize);

            return result;
        }
    }
}
