using System;
using System.Text.Json;
using Atles.Domain.Models.Users;

namespace Atles.Domain.Models
{
    public class HistoryItem
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

        public HistoryItem()
        {
        }

        public HistoryItem(
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
            if(data != null) Data = JsonSerializer.Serialize(data);
        }

        public HistoryItem(
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
            if (data != null) Data = JsonSerializer.Serialize(data);
        }
    }
}
