using System;
using Atlas.Domain;

namespace Atlas.Models.Public
{
    public class MemberPageModel
    {
        public MemberModel Member { get; set; } = new MemberModel();

        public PaginatedData<SearchPostModel> Posts { get; set; } = new PaginatedData<SearchPostModel>();
    }

    public class MemberModel
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public int TotalTopics { get; set; }
        public int TotalReplies { get; set; }
        public string GravatarHash { get; set; }
        public StatusType Status { get; set; }
    }
}
