using Newtonsoft.Json;
using System;
using Atlas.Domain.Members;

namespace Atlas.Domain
{
    public class Event
    {
        public Guid SiteId { get; private set; }
        public Guid Id { get; private set; } = Guid.NewGuid();
        public DateTime TimeStamp { get; private set; } = DateTime.UtcNow;
        public Guid TargetId { get; private set; }
        public string TargetType { get; private set; }
        public string Type { get; private set; }
        public string Data { get; private set; }
        
        public Guid? MemberId { get; private set; }

        public virtual Member Member { get; set; }

        public Event()
        {
            
        }

        public Event(Guid siteId, 
            Guid? memberId, 
            EventType eventType, 
            Type targetType, 
            Guid targetId, 
            object data = null)
        {
            Type = eventType.ToString();
            TargetType = targetType.Name;
            TargetId = targetId;
            MemberId = memberId;
            SiteId = siteId;
            Data = JsonConvert.SerializeObject(data);
        }

        public Event(EventBase @event)
        {
            Type = @event.GetType().Name;
            TargetType = @event.TargetType;
            TargetId = @event.TargetId;
            MemberId = @event.MemberId;
            SiteId = @event.SiteId;
            Data = JsonConvert.SerializeObject(@event);
        }
    }
}
