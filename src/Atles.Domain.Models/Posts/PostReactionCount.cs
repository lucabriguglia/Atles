using Atles.Domain.PostReactions;
using Docs.Attributes;
using System;
using Atles.Domain.Models;
using Atles.Domain.Models.Posts;

namespace Atles.Domain.Posts
{
    [DocTarget(Consts.DocsContextForum)]
    public class PostReactionCount
    {
        public Guid PostId { get; private set; }

        public PostReactionType Type { get; private set; }

        public int Count { get; private set; }

        public virtual Post Post { get; set; }

        public PostReactionCount()
        {
        }

        public PostReactionCount(Guid postId, PostReactionType type)
        {
            PostId = postId;
            Type = type;
            Count = 1;
        }

        /// <summary>
        /// Increases count by 1.
        /// </summary>
        public void IncreaseCount()
        {
            Count += 1;
        }

        /// <summary>
        /// Decreases count by 1.
        /// If the resulting number is less than zero, the value is set to zero.
        /// </summary>
        public void DecreaseCount()
        {
            Count -= 1;

            if (Count < 0)
            {
                Count = 0;
            }
        }
    }
}
