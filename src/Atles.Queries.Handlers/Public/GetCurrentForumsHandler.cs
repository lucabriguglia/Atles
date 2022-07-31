using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atles.Core;
using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain;
using Atles.Models.Public;
using Atles.Queries.Public;
using Microsoft.EntityFrameworkCore;

namespace Atles.Queries.Handlers.Public
{
    public class GetCurrentForumsHandler : IQueryHandler<GetCurrentForums, IList<CurrentForumModel>>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IDispatcher _dispatcher;
        public GetCurrentForumsHandler(AtlesDbContext dbContext, ICacheManager cacheManager, IDispatcher sender)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
            _dispatcher = sender;
        }

        public async Task<QueryResult<IList<CurrentForumModel>>> Handle(GetCurrentForums query)
        {
            // TODO: To be moved to a service
            var getCurrentSiteResult = await _dispatcher.Get(new GetCurrentSite());
            var site = getCurrentSiteResult.AsT0;

            return await _cacheManager.GetOrSetAsync(CacheKeys.CurrentForums(site.Id), async () =>
            {
                var forums = await _dbContext.Forums
                    .Where(x => x.Category.SiteId == site.Id && x.Status == ForumStatusType.Published)
                    .Select(x => new
                    {
                        x.Id,
                        PermissionSetId = x.PermissionSetId ?? x.Category.PermissionSetId
                    })
                    .ToListAsync();

                return forums.Select(forum => new CurrentForumModel
                {
                    Id = forum.Id,
                    PermissionSetId = forum.PermissionSetId
                }).ToList();
            });
        }
    }
}
