using System;
using System.Collections.Generic;

namespace Atlify.Models.Admin.Members
{
    public class ActivityPageModel
    {
        public MemberModel Member { get; set; } = new MemberModel();

        public PaginatedData<EventModel> Events { get; set; } = new PaginatedData<EventModel>();

        public class MemberModel
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