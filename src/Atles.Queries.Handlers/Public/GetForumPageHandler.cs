﻿using System.Threading.Tasks;
using Atles.Core;
using Atles.Core.Queries;
using Atles.Data;
using Atles.Domain;
using Atles.Models.Public;
using Atles.Queries.Public;
using Microsoft.EntityFrameworkCore;

namespace Atles.Queries.Handlers.Public
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
