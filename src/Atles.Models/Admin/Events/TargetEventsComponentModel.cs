namespace Atles.Models.Admin.Events
{
    public class TargetEventsComponentModel
    {
        public Guid Id { get; set; }
        public string Type { get; set; }

        public IList<EventModel> Events { get; set; } = new List<EventModel>();

        public class EventModel
        {
            public Guid Id { get; set; }
            public DateTime TimeStamp { get; set; }
            public string Type { get; set; }
            public Guid? UserId { get; set; }
            public string UserName { get; set; }
            public Dictionary<string, string> Data { get; set; }
        }
    }
}
