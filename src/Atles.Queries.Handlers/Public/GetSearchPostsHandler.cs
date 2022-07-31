using System.Linq;
using System.Threading.Tasks;
using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Data;
using Atles.Data.Extensions;
using Atles.Domain;
using Atles.Models;
using Atles.Models.Public;
using Atles.Queries.Public;
using Markdig;
using Microsoft.EntityFrameworkCore;

namespace Atles.Queries.Handlers.Public
{
    public class GetSearchPostsHandler : IQueryHandler<GetSearchPosts, PaginatedData<SearchPostModel>>
    {
        private readonly AtlesDbContext _dbContext;

        public GetSearchPostsHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<QueryResult<PaginatedData<SearchPostModel>>> Handle(GetSearchPosts query)
        {
            var postsQuery = _dbContext.Posts
                .Where(x =>
                    query.AccessibleForumIds.Contains(x.ForumId) &&
                    x.Status == PostStatusType.Published &&
                    (x.Topic == null || x.Topic.Status == PostStatusType.Published));

            if (query.Options.SearchIsDefined())
            {
                postsQuery = postsQuery.Where(x => x.Title.Contains(query.Options.Search) || x.Content.Contains(query.Options.Search));
            }

            postsQuery = query.Options.OrderByIsDefined()
                ? postsQuery.OrderBy(query.Options)
                : postsQuery.OrderByDescending(x => x.CreatedOn);

            if (query.UserId != null)
            {
                postsQuery = postsQuery.Where(x => x.CreatedBy == query.UserId);
            }

            var posts = await postsQuery
                .Skip(query.Options.Skip)
                .Take(query.Options.PageSize)
                .Select(p => new
                {
                    p.Id,
                    TopicId = p.TopicId ?? p.Id,
                    IsTopic = p.TopicId == null,
                    Title = p.Title ?? p.Topic.Title,
                    Slug = p.Slug ?? p.Topic.Slug,
                    p.Content,
                    TimeStamp = p.CreatedOn,
                    UserId = p.CreatedBy,
                    UserDisplayName = p.CreatedByUser.DisplayName,
                    p.ForumId,
                    ForumName = p.Forum.Name,
                    ForumSlug = p.Forum.Slug
                })
                .ToListAsync();

            var items = posts.Select(post => new SearchPostModel
            {
                Id = post.Id,
                TopicId = post.TopicId,
                IsTopic = post.IsTopic,
                Title = post.Title,
                Slug = post.Slug,
                Content = Markdown.ToHtml(post.Content),
                TimeStamp = post.TimeStamp,
                UserId = post.UserId,
                UserDisplayName = post.UserDisplayName,
                ForumId = post.ForumId,
                ForumName = post.ForumName,
                ForumSlug = post.ForumSlug
            }).ToList();

            var totalRecords = await postsQuery.CountAsync();

            return new PaginatedData<SearchPostModel>(items, totalRecords, query.Options.PageSize);
        }
    }
}
