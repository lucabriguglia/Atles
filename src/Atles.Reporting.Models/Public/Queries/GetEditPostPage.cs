using System;
using Atles.Core.Queries;

namespace Atles.Reporting.Models.Public.Queries
{
    public class GetEditPostPage : QueryBase<PostPageModel>
    {
        public Guid ForumId { get; set; }
        public Guid TopicId { get; set; }
        public Guid UserId { get; set; }
    }
}
