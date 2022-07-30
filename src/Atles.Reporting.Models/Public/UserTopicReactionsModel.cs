using System;
using System.Collections.Generic;
using System.Linq;
using Atles.Domain;

namespace Atles.Reporting.Models.Public;

public class UserTopicReactionsModel
{
    public Dictionary<Guid, PostReactionType> PostReactions { get; set; } = new();

    public bool HasReactedTo(Guid postId) => PostReactions.Any(x => x.Key == postId);
    public KeyValuePair<Guid, PostReactionType> ReactionTo(Guid postId) => PostReactions.FirstOrDefault(x => x.Key == postId);
}