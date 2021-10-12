using System;
using Atles.Domain.Users;
using Newtonsoft.Json;

namespace Atles.Domain
{
    public class Event
    {
        public Guid Id { get; private set; } = Guid.NewGuid();        
        public string Type { get; private set; }

        public Guid TargetId { get; private set; }
        public string TargetType { get; private set; }

        public Guid SiteId { get; private set; }
        public Guid? UserId { get; private set; }

        public string Data { get; private set; }

        public DateTime TimeStamp { get; private set; } = DateTime.UtcNow;

        public virtual User User { get; set; }

        public Event()
        {
        }

        public Event(
            EventType eventType,
            Guid targetId,
            Type targetType,
            Guid siteId,
            Guid? userId,
            object data = null)
        {
            Type = eventType.ToString();
            TargetId = targetId;
            TargetType = targetType.Name;
            SiteId = siteId;
            UserId = userId;
            if(data != null) Data = JsonConvert.SerializeObject(data);
        }

        public Event(
            Guid siteId,
            Guid? userId,
            EventType eventType,
            Type targetType,
            Guid targetId,
            object data = null)
        {
            Type = eventType.ToString();
            TargetId = targetId;
            TargetType = targetType.Name;
            SiteId = siteId;
            UserId = userId;
            if (data != null) Data = JsonConvert.SerializeObject(data);
        }
    }
}
