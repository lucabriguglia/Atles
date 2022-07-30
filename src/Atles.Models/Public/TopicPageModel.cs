using System.ComponentModel.DataAnnotations;
using Atles.Domain;

namespace Atles.Models.Public
{
    public class TopicPageModel
    {
        public ForumModel Forum { get; set; } = new();
        public TopicModel Topic { get; set; } = new();
        public ReplyModel Answer { get; set; } = new();
        public PaginatedData<ReplyModel> Replies { get; set; } = new();
        public PostModel Post { get; set; } = new();
        public PermissionsModel Permissions { get; set; } = new();

        public class PermissionsModel
        {
            public bool CanReply { get; set; }
            public bool CanEdit { get; set; }
            public bool CanDelete { get; set; }
            public bool CanModerate { get; set; }
            public bool CanReact { get; set; }
        }

        public class ForumModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Slug { get; set; }
        }

        public class TopicModel : PostReactionBase
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
            public bool Subscribed { get; set; }
        }

        public class ReplyModel : PostReactionBase
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
        }

        public abstract class PostReactionBase
        {
            public IList<ReactionModel> Reactions { get; set; } = new List<ReactionModel>();

            public void AddReaction(PostReactionType type)
            {
                var reaction = Reactions.FirstOrDefault(x => x.Type == type);

                if (reaction != null)
                {
                    reaction.Count++;
                }
                else
                {
                    Reactions.Add(new ReactionModel { Type = type, Count = 1 });
                }
            }

            public void RemoveReaction(PostReactionType type)
            {
                var reaction = Reactions.FirstOrDefault(x => x.Type == type);

                if (reaction != null)
                {
                    reaction.Count--;
                }
            }
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
