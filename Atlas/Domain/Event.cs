//using System;
//using System.Text.Json;

//namespace Atlas.Domain
//{
//    public class Event
//    {
//        public Guid Id { get;}
//        public Guid TargetId { get; }
//        public string TargetSnapshot { get; }
//        public string Type { get; }
//        public string Data { get; }
//        public DateTime TimeStamp { get; }
//        public string UserId { get; }

//        public Event(Guid targetId, object target, string type, string userId)
//        {
//            Id = Id;
//            TimeStamp = DateTime.UtcNow;
//            TargetId = targetId;
//            TargetSnapshot = JsonSerializer.Serialize(target);
//            Type = type;
//            UserId = userId;
//        }
//    }
//}
