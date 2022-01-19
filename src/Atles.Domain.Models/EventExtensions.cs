using Atles.Infrastructure.Events;
using Newtonsoft.Json;

namespace Atles.Domain.Models
{
    /// <summary>
    /// Extensions for IEvent
    /// </summary>
    public static class EventExtensions
    {
        /// <summary>
        /// Converts IEvent to HistoryItem
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public static HistoryItem ToHistoryItem(this IEvent @event)
        {
            return new HistoryItem
            {
                Id = @event.Id,
                TimeStamp = @event.TimeStamp,

                Type = @event.GetType().Name,
                Data = JsonConvert.SerializeObject(@event),

                TargetId = @event.TargetId,
                TargetType = @event.TargetType,
                
                SiteId = @event.SiteId,
                UserId = @event.UserId
            };
        }
    }
}