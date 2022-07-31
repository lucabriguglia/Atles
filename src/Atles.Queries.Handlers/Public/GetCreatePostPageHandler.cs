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
    public class GetCreatePostPageHandler : IQueryHandler<GetCreatePostPage, PostPageModel>
    {
        private readonly AtlesDbContext _dbContext;

        public GetCreatePostPageHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<QueryResult<PostPageModel>> Handle(GetCreatePostPage query)
        {
            var forum = await _dbContext.Forums
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x =>
                    x.Id == query.ForumId &&
                    x.Category.SiteId == query.SiteId &&
                    x.Status == ForumStatusType.Published);

            if (forum == null)
            {
                return null;
            }

            var result = new PostPageModel
            {
                Forum = new PostPageModel.ForumModel
                {
                    Id = forum.Id,
                    Name = forum.Name,
                    Slug = forum.Slug
                },
                Topic = new PostPageModel.TopicModel
                {
                    Subscribe = true
                }
            };

            return result;
        }
    }
}
