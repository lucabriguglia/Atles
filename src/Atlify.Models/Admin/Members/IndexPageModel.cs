using System;
using Atlify.Domain;

namespace Atlify.Models.Admin.Members
{
    public class IndexPageModel
    {
        public PaginatedData<MemberModel> Members { get; set; } = new PaginatedData<MemberModel>();

        public class MemberModel
        {
            public Guid Id { get; set; }
            public string DisplayName { get; set; }
            public string Email { get; set; }
            public int TotalTopics { get; set; }
            public int TotalReplies { get; set; }
            public StatusType Status { get; set; }
        }
    }
}
