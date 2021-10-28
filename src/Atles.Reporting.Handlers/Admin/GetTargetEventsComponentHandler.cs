using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Reporting.Models.Admin.Events;
using Atles.Reporting.Models.Admin.Events.Queries;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using OpenCqrs.Queries;

namespace Atles.Reporting.Handlers.Admin
{
    public class GetTargetEventsComponentHandler : IQueryHandler<GetTargetEventsComponent, TargetEventsComponentModel>
    {
        private readonly AtlesDbContext _dbContext;

        public GetTargetEventsComponentHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TargetEventsComponentModel> Handle(GetTargetEventsComponent query)
        {
            var events = await _dbContext.Events
                .Include(x => x.User)
                .Where(x => x.SiteId == query.SiteId && x.TargetId == query.Id)
                .OrderByDescending(x => x.TimeStamp)
                .ToListAsync();

            if (events.Count == 0)
            {
                return new TargetEventsComponentModel();
            }

            var result = new TargetEventsComponentModel
            {
                Id = events[0].TargetId,
                Type = events[0].TargetType
            };

            foreach (var @event in events)
            {
                var model = new TargetEventsComponentModel.EventModel
                {
                    Id = @event.Id,
                    Type = @event.Type,
                    TimeStamp = @event.TimeStamp,
                    UserId = @event.UserId,
                    UserName = @event.User?.DisplayName ?? "<system>"
                };

                if (!string.IsNullOrWhiteSpace(@event.Data) && @event.Data != "null")
                {
                    var parsedData = JObject.Parse(@event.Data);

                    var data = new Dictionary<string, string>();

                    foreach (var x in parsedData)
                    {
                        if (x.Key != nameof(@event.Id) &&
                            x.Key != nameof(@event.TargetId) &&
                            x.Key != nameof(@event.TargetType) &&
                            x.Key != nameof(@event.SiteId) &&
                            x.Key != nameof(@event.UserId))
                        {
                            var value = !string.IsNullOrWhiteSpace(x.Value.ToString())
                                ? x.Value.ToString()
                                : "<null>";

                            data.Add(x.Key, value);
                        }
                    }

                    model.Data = data;
                }

                result.Events.Add(model);
            }

            return result;
        }
    }
}
