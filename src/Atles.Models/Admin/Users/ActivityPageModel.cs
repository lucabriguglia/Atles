namespace Atles.Models.Admin.Users
{
    public class ActivityPageModel
    {
        public UserModel User { get; set; } = new UserModel();

        public PaginatedData<EventModel> Events { get; set; } = new PaginatedData<EventModel>();

        public class UserModel
        {
            public Guid Id { get; set; }
            public string DisplayName { get; set; }
        }

        public class EventModel
        {
            public Guid Id { get; set; }
            public string Type { get; set; }
            public Guid TargetId { get; set; }
            public string TargetType { get; set; }
            public DateTime TimeStamp { get; set; }
            public Dictionary<string, string> Data { get; set; }
        }
    }
}