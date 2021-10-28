using Atles.Data;
using Microsoft.EntityFrameworkCore;
using OpenCqrs;
using OpenCqrs.Queries;
using System.Threading.Tasks;
using Atles.Domain.Models.Forums;
using Atles.Reporting.Models.Public;
using Atles.Reporting.Models.Public.Queries;

namespace Atles.Reporting.Handlers.Public
{
    public class GetForumPageHandler : IQueryHandler<GetForumPage, ForumPageModel>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly IDispatcher _dispatcher;

        public GetForumPageHandler(AtlesDbContext dbContext, IDispatcher sender)
        {
            _dbContext = dbContext;
            _dispatcher = sender;
        }

        public async Task<ForumPageModel> Handle(GetForumPage query)
        {
            var forum = await _dbContext.Forums
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x =>
                    x.Slug == query.Slug &&
                    x.Category.SiteId == query.SiteId &&
                    x.Status == ForumStatusType.Published);

            if (forum == null)
            {
                return null;
            }

            var result = new ForumPageModel
            {
                Forum = new ForumPageModel.ForumModel
                {
                    Id = forum.Id,
                    Name = forum.Name,
                    Description = forum.Description,
                    Slug = forum.Slug
                },
                Topics = await _dispatcher.Get(new GetForumPageTopics { ForumId = forum.Id, Options = query.Options })
            };

            return result;
        }
    }
}
