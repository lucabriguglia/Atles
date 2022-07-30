using System;

namespace Atles.Core.Events;

public abstract class EventBase : IEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TargetId { get; set; }
    public string TargetType { get; set; }

    public Guid SiteId { get; set; }
    public Guid? UserId { get; set; }

    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

    public bool ShouldSerializeId() => false;
    public bool ShouldSerializeTargetId() => false;
    public bool ShouldSerializeTargetType() => false;
    public bool ShouldSerializeSiteId() => false;
    public bool ShouldSerializeUserId() => false;
    public bool ShouldSerializeTimeStamp() => false;
}