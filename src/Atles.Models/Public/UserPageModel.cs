using Atles.Domain;

namespace Atles.Models.Public
{
    public class UserPageModel
    {
        public UserModel User { get; set; } = new UserModel();

        public PaginatedData<SearchPostModel> Posts { get; set; } = new PaginatedData<SearchPostModel>();
    }

    public class UserModel
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public int TotalTopics { get; set; }
        public int TotalReplies { get; set; }
        public string GravatarHash { get; set; }
        public UserStatusType Status { get; set; }
    }
}
