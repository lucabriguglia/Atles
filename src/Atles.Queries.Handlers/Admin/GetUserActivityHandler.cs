using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Core.Results.Types;
using Atles.Data;
using Atles.Models;
using Atles.Models.Admin.Users;
using Atles.Queries.Admin;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace Atles.Queries.Handlers.Admin;

public class GetUserActivityHandler : IQueryHandler<GetUserActivity, ActivityPageModel>
{
    private readonly AtlesDbContext _dbContext;

    public GetUserActivityHandler(AtlesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<QueryResult<ActivityPageModel>> Handle(GetUserActivity request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.UserId);

        if (user == null)
        {
            return new Failure(FailureType.NotFound, "User", $"User with id {request.UserId} not found.");
        }

        var result = new ActivityPageModel
        {
            User = new ActivityPageModel.UserModel
            {
                Id = user.Id,
                DisplayName = user.DisplayName
            }
        };

        var query = _dbContext.Events.Where(x => x.SiteId == request.SiteId && x.UserId == request.UserId);

        if (!string.IsNullOrWhiteSpace(request.Options.Search))
        {
            query = query.Where(x => x.Type.Contains(request.Options.Search) ||
                                     x.TargetType.Contains(request.Options.Search) ||
                                     x.Data.Contains(request.Options.Search));
        }

        var events = await query
            .OrderByDescending(x => x.TimeStamp)
            .Skip(request.Options.Skip)
            .Take(request.Options.PageSize)
            .ToListAsync();

        var items = new List<ActivityPageModel.EventModel>();

        foreach (var @event in events)
        {
            var model = new ActivityPageModel.EventModel
            {
                Id = @event.Id,
                Type = @event.Type,
                TargetId = @event.TargetId,
                TargetType = @event.TargetType,
                TimeStamp = @event.TimeStamp
            };

            var data = new Dictionary<string, string>
            {
                {nameof(@event.TargetId), @event.TargetId.ToString()}
            };

            if (!string.IsNullOrWhiteSpace(@event.Data) && @event.Data != "null")
            {
                var parsedData = JObject.Parse(@event.Data);

                foreach (var x in parsedData)
                {
                    if (x.Key == nameof(@event.Id) ||
                        x.Key == nameof(@event.TargetId) ||
                        x.Key == nameof(@event.TargetType) ||
                        x.Key == nameof(@event.SiteId) ||
                        x.Key == nameof(@event.UserId))
                        continue;

                    var value = !string.IsNullOrWhiteSpace(x.Value.ToString())
                        ? x.Value.ToString()
                        : "<null>";

                    data.Add(x.Key, value);
                }
            }

            model.Data = data;

            items.Add(model);
        }

        var totalRecords = await query.CountAsync();

        result.Events = new PaginatedData<ActivityPageModel.EventModel>(items, totalRecords, request.Options.PageSize);

        return result;
    }
}
