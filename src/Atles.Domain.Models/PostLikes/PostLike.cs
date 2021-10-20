using Atles.Domain.Posts;
using Atles.Domain.Users;
using Docs.Attributes;
using System;

namespace Atles.Domain.PostLikes
{
    [DocTarget(Consts.DocsContextForum)]
    public class PostLike
    {
        public Guid PostId { get; private set; }

        public Guid UserId { get; private set; }

        public bool Like { get; private set; }

        public DateTime TimeStamp { get; private set; }

        public virtual Post Post { get; set; }

        public virtual User User { get; set; }

        public PostLike()
        {
        }

        public PostLike(Guid postId, Guid userId, bool like = true)
        {
            PostId = postId;
            UserId = userId;
            Like = like;
            TimeStamp = DateTime.UtcNow;
        }
    }
}
