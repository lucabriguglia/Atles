using Atlas.Shared;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Shared.Admin.Events.Models;

namespace Atlas.Data.Builders.Admin
{
    public class EventModelBuilder : IEventModelBuilder
    {
        private readonly AtlasDbContext _dbContext;

        public EventModelBuilder(AtlasDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TargetEventsComponentModel> BuildTargetModelAsync(Guid siteId, Guid id)
        {
            var events = await _dbContext.Events
                .Include(x => x.Member)
                .Where(x => x.SiteId == siteId && x.TargetId == id)
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
                    MemberId = @event.MemberId,
                    MemberName = @event.Member?.DisplayName ?? "<system>"
                };

                var parsedData = JObject.Parse(@event.Data);

                var data = new Dictionary<string, string>();

                foreach (var x in parsedData)
                {
                    if (x.Key != nameof(@event.Id) && 
                        x.Key != nameof(@event.TargetId) &&
                        x.Key != nameof(@event.TargetType) &&
                        x.Key != nameof(@event.SiteId) &&
                        x.Key != nameof(@event.MemberId))
                    {
                        var value = !string.IsNullOrWhiteSpace(x.Value.ToString()) 
                            ? x.Value.ToString()
                            : "<null>";

                        data.Add(x.Key, value);
                    }
                }

                model.Data = data;

                result.Events.Add(model);
            }

            return result;
        }
    }
}
