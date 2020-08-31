using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Atlas.Domain.Users;

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

        [Column("UserId")]
        public Guid? UserId { get; private set; }

        public virtual User User { get; set; }

        public Event()
        {
            
        }

        public Event(Guid siteId, 
            Guid? userId, 
            EventType eventType, 
            Type targetType, 
            Guid targetId, 
            object data = null)
        {
            Type = eventType.ToString();
            TargetType = targetType.Name;
            TargetId = targetId;
            UserId = userId;
            SiteId = siteId;
            Data = JsonConvert.SerializeObject(data);
        }
    }
}
