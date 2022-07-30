using Atles.Core.Queries;

namespace Atles.Queries.Public
{
    public class GetTopicSlug : QueryBase<string>
    {
        public Guid TopicId { get; set; }
    }
}
