using System;
using Docs.Attributes;

namespace Atles.Domain
{
    [DocTarget(Consts.DocsContextForum)]
    public class PostReactionSummary
    {
        public Guid PostId { get; private set; }

        public PostReactionType Type { get; private set; }

        public int Count { get; private set; }

        public virtual Post Post { get; set; }

        public PostReactionSummary()
        {
        }

        public PostReactionSummary(Guid postId, PostReactionType type)
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
