using System;

namespace Atles.Core.Events;

public interface IEvent
{
    Guid Id { get; set; }

    Guid TargetId { get; set; }
    string TargetType { get; set; }

    Guid SiteId { get; set; }
    Guid? UserId { get; set; }

    DateTime TimeStamp { get; set; }
}