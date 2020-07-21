using System;

namespace Atlas.Domain
{
    public abstract class EventBase
    {
        public Guid SiteId { get; set; }
        public Guid? MemberId { get; set; }
        public Guid TargetId { get; set; }
        public string TargetType { get; set; }
    }
}
