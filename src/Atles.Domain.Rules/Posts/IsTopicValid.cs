using Atles.Core.Queries;

namespace Atles.Domain.Rules.Posts
{
    public class IsTopicValid : QueryBase<bool>
    {
        public Guid Id { get; set; }
        public Guid ForumId { get; set; }
    }
}
