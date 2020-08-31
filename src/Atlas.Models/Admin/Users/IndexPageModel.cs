using System;
using Atlas.Domain;

namespace Atlas.Models.Admin.Users
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

        public PaginatedData<UserModel> Users { get; set; } = new PaginatedData<UserModel>();

        public class UserModel
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
