using System;
using Atles.Infrastructure.Queries;

namespace Atles.Reporting.Models.Public.Queries
{
    public class GetCreatePostPage : QueryBase<PostPageModel>
    {
        public Guid ForumId { get; set; }
    }
}
