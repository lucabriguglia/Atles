using System;
using Atles.Infrastructure.Queries;

namespace Atles.Reporting.Models.Public.Queries;

public class GetUserTopicReactions : QueryBase<UserTopicReactionsModel>
{
    public Guid UserId { get; set; }
    public Guid TopicId { get; set; }
}