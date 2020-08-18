using System;
using Atlify.Models.Public.Search;
using Atlify.Domain;

namespace Atlify.Models.Public.Members
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
