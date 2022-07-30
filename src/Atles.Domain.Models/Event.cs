using System;

namespace Atles.Domain
{
    public class Event
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

        public string Type { get; set; }
        public string Data { get; set; }

        public Guid TargetId { get; set; }
        public string TargetType { get; set; }

        public Guid SiteId { get; set; }
        public Guid? UserId { get; set; }

        public virtual User User { get; set; }
    }
}
