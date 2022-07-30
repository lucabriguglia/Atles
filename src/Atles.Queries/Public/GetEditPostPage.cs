using Atles.Core.Queries;
using Atles.Models.Public;

namespace Atles.Queries.Public
{
    public class GetEditPostPage : QueryBase<PostPageModel>
    {
        public Guid ForumId { get; set; }
        public Guid TopicId { get; set; }
        public Guid UserId { get; set; }
    }
}
