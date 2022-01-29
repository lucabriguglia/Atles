using System;
using System.Collections.Generic;
using System.Linq;
using Atles.Domain.Models.PostReactions;

namespace Atles.Reporting.Models.Public;

public class UserInfoModel
{
    public Dictionary<Guid, PostReactionType> Reactions { get; set; } = new();

    public bool HasReactedTo(Guid postId) => Reactions.Any(x => x.Key == postId);
    public KeyValuePair<Guid, PostReactionType> ReactionTo(Guid postId) => Reactions.FirstOrDefault(x => x.Key == postId);
}