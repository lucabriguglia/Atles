using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Atles.Domain.Models.PostReactions;
using Atles.Reporting.Models.Shared;

namespace Atles.Reporting.Models.Public
{
    public class TopicPageModel
    {
        public ForumModel Forum { get; set; } = new();
        public TopicModel Topic { get; set; } = new();
        public ReplyModel Answer { get; set; } = new();
        public PaginatedData<ReplyModel> Replies { get; set; } = new();
        public PostModel Post { get; set; } = new();

        public bool CanReply { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanModerate { get; set; }
        public bool CanReact { get; set; }

        public class ForumModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Slug { get; set; }
        }

        public class TopicModel
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public string Slug { get; set; }
            public string Content { get; set; }
            public Guid UserId { get; set; }
            public string UserDisplayName { get; set; }
            public DateTime TimeStamp { get; set; }
            public string IdentityUserId { get; set; }
            public string GravatarHash { get; set; }
            public bool Pinned { get; set; }
            public bool Locked { get; set; }
            public bool HasAnswer { get; set; }
            public IList<ReactionModel> Reactions { get; set; } = new List<ReactionModel>();
            public bool Reacted { get; set; }
        }

        public class ReplyModel
        {
            public Guid Id { get; set; }
            public string Content { get; set; }
            public string OriginalContent { get; set; }
            public string IdentityUserId { get; set; }
            public Guid UserId { get; set; }
            public string UserDisplayName { get; set; }
            public DateTime TimeStamp { get; set; }
            public string GravatarHash { get; set; }
            public bool IsAnswer { get; set; }
            public IList<ReactionModel> Reactions { get; set; } = new List<ReactionModel>();
            public bool Reacted { get; set; }
        }

        public class ReactionModel
        {
            public PostReactionType Type { get; set; }
            public int Count { get; set; }
        }

        public class PostModel
        {
            public Guid? Id { get; set; }

            [Required]
            public string Content { get; set; }

            public Guid UserId { get; set; }
        }
    }
}
