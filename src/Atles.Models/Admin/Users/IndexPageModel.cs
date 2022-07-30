using Atles.Domain;

namespace Atles.Models.Admin.Users
{
    public class IndexPageModel
    {
        public UserStatusType[] Status { get; } =
        {
            UserStatusType.Active,
            UserStatusType.Pending,
            UserStatusType.Suspended,
            UserStatusType.Deleted
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
            public string IdentityUserId { get; set; }
            public string DisplayName { get; set; }
            public string Email { get; set; }
            public int TotalTopics { get; set; }
            public int TotalReplies { get; set; }
            public UserStatusType Status { get; set; }
            public DateTime TimeStamp { get; set; }
        }
    }
}
