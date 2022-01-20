using System;
using Atles.Infrastructure.Commands;

namespace Atles.Infrastructure.Events
{
    public abstract class EventBase : IEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid TargetId { get; set; }
        public string TargetType { get; set; }

        public Guid SiteId { get; set; }
        public Guid UserId { get; set; }

        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    }
}