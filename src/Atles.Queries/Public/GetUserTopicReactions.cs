using Atles.Core.Queries;
using Atles.Models.Public;

namespace Atles.Queries.Public;

public class GetUserTopicReactions : QueryBase<UserTopicReactionsModel>
{
    public Guid UserId { get; set; }
    public Guid TopicId { get; set; }
}