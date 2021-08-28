using System;

namespace Atles.Domain
{
    public abstract class EventBase : IEvent
    {
        public Guid Id { get; set; }
        public Guid StoreId { get; set; }
        public string UserId { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    }
}
