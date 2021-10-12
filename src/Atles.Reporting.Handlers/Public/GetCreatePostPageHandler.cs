using Atles.Data;
using Atles.Domain.Forums;
using Atles.Models.Public.Posts;
using Atles.Reporting.Public.Queries;
using Microsoft.EntityFrameworkCore;
using OpenCqrs.Queries;
using System.Threading.Tasks;

namespace Atles.Reporting.Handlers.Public
{
    public class GetCreatePostPageHandler : IQueryHandler<GetCreatePostPage, PostPageModel>
    {
        private readonly AtlesDbContext _dbContext;

        public GetCreatePostPageHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PostPageModel> Handle(GetCreatePostPage query)
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
                }
            };

            return result;
        }
    }
}
