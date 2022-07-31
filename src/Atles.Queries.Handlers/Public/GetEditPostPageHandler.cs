using System.Threading.Tasks;
using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Data;
using Atles.Domain;
using Atles.Models.Public;
using Atles.Queries.Public;
using Microsoft.EntityFrameworkCore;

namespace Atles.Queries.Handlers.Public
{
    public class GetEditPostPageHandler : IQueryHandler<GetEditPostPage, PostPageModel>
    {
        private readonly AtlesDbContext _dbContext;

        public GetEditPostPageHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<QueryResult<PostPageModel>> Handle(GetEditPostPage query)
        {
            var topic = await _dbContext.Posts
                .Include(x => x.Forum).ThenInclude(x => x.Category)
                .Include(x => x.CreatedByUser)
                .FirstOrDefaultAsync(x =>
                    x.TopicId == null &&
                    x.Forum.Category.SiteId == query.SiteId &&
                    x.Forum.Id == query.ForumId &&
                    x.Id == query.TopicId &&
                    x.Status == PostStatusType.Published);

            if (topic == null)
            {
                return null;
            }

            var subscription = await _dbContext.Subscriptions.FirstOrDefaultAsync(x => x.UserId == query.UserId && x.ItemId == query.TopicId);

            var result = new PostPageModel
            {
                Forum = new PostPageModel.ForumModel
                {
                    Id = topic.Forum.Id,
                    Name = topic.Forum.Name,
                    Slug = topic.Forum.Slug
                },
                Topic = new PostPageModel.TopicModel
                {
                    Id = topic.Id,
                    Title = topic.Title,
                    Slug = topic.Slug,
                    Content = topic.Content,
                    UserId = topic.CreatedByUser.Id,
                    Locked = topic.Locked,
                    Subscribe = subscription != null
                }
            };

            return result;
        }
    }
}
