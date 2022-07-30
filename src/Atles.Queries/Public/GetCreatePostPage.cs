using Atles.Core.Queries;
using Atles.Models.Public;

namespace Atles.Queries.Public
{
    public class GetCreatePostPage : QueryBase<PostPageModel>
    {
        public Guid ForumId { get; set; }
    }
}
