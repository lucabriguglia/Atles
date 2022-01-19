using System;
using System.Text.Json.Serialization;

namespace Atles.Infrastructure.Events
{
    public class EventBase : IEvent
    {
        [JsonIgnore]
        public Guid Id { get; set; } = Guid.NewGuid();

        [JsonIgnore]
        public Guid TargetId { get; set; }
        [JsonIgnore]
        public string TargetType { get; set; }

        [JsonIgnore]
        public Guid SiteId { get; set; }
        [JsonIgnore]
        public Guid UserId { get; set; }

        [JsonIgnore]
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    }
}