using System;
using Docs.Attributes;

namespace Atles.Domain;

[DocTarget(Consts.DocsContextForum)]
public class PostReaction
{
    public Guid PostId { get; private set; }

    public Guid UserId { get; private set; }

    public PostReactionType Type { get; private set; }

    public DateTime TimeStamp { get; private set; }

    public virtual Post Post { get; set; }

    public virtual User User { get; set; }

    public PostReaction()
    {
    }

    public PostReaction(Guid postId, Guid userId, PostReactionType type)
    {
        PostId = postId;
        UserId = userId;
        Type = type;
        TimeStamp = DateTime.UtcNow;
    }
}