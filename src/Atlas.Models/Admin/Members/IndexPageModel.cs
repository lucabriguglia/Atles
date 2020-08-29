using System;
using Atlas.Domain;

namespace Atlas.Models.Admin.Members
{
    public class IndexPageModel
    {
        public StatusType[] Status { get; } =
        {
            StatusType.Active,
            StatusType.Pending,
            StatusType.Suspended,
            StatusType.Deleted
        };

        public string[] SortBy { get; } =
        {
            "DisplayName-Asc",
            "DisplayName-Desc",
            "Email-Asc",
            "Email-Desc",
            "TopicsCount-Asc",
            "TopicsCount-Desc",
            "RepliesCount-Asc",
            "RepliesCount-Desc",
            "TimeStamp-Desc",
            "TimeStamp-Asc"
        };

        public PaginatedData<MemberModel> Members { get; set; } = new PaginatedData<MemberModel>();

        public class MemberModel
        {
            public Guid Id { get; set; }
            public string DisplayName { get; set; }
            public string Email { get; set; }
            public int TotalTopics { get; set; }
            public int TotalReplies { get; set; }
            public StatusType Status { get; set; }
            public DateTime TimeStamp { get; set; }
        }
    }
}
