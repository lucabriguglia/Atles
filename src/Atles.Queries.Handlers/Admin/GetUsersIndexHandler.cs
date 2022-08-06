using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Data;
using Atles.Data.Extensions;
using Atles.Domain;
using Atles.Models;
using Atles.Models.Admin.Users;
using Atles.Queries.Admin;
using Microsoft.EntityFrameworkCore;

namespace Atles.Queries.Handlers.Admin;

public class GetUsersIndexHandler : IQueryHandler<GetUsersIndex, IndexPageModel>
{
    private readonly AtlesDbContext _dbContext;

    public GetUsersIndexHandler(AtlesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<QueryResult<IndexPageModel>> Handle(GetUsersIndex request)
    {
        var result = new IndexPageModel();

        var query = _dbContext.Users.Where(x => true);

        if (request.Options.SearchIsDefined())
        {
            query = query.Where(x => x.DisplayName.Contains(request.Options.Search) || x.Email.Contains(request.Options.Search));
        }

        query = request.Options.OrderByIsDefined()
            ? query.OrderBy(request.Options)
            : query.OrderBy(x => x.DisplayName);

        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse(request.Status, out UserStatusType memberStatus))
        {
            query = query.Where(x => x.Status == memberStatus);
        }

        var users = await query
            .Skip(request.Options.Skip)
            .Take(request.Options.PageSize)
            .ToListAsync();

        var items = users.Select(user => new IndexPageModel.UserModel
            {
                Id = user.Id,
                IdentityUserId = user.IdentityUserId,
                DisplayName = user.DisplayName,
                Email = user.Email,
                TotalTopics = user.TopicsCount,
                TotalReplies = user.RepliesCount,
                Status = user.Status,
                TimeStamp = user.TimeStamp
            })
            .ToList();

        var countQuery = _dbContext.Users.Where(x => true);

        if (!string.IsNullOrWhiteSpace(request.Options.Search))
        {
            countQuery = countQuery.Where(x => x.DisplayName.Contains(request.Options.Search) || x.Email.Contains(request.Options.Search));
        }

        var totalRecords = await countQuery.CountAsync();

        result.Users = new PaginatedData<IndexPageModel.UserModel>(items, totalRecords, request.Options.PageSize);

        return result;
    }
}
