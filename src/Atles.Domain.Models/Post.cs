using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Docs.Attributes;

namespace Atles.Domain
{
    /// <summary>
    /// A forum post is a message created in a forum.
    /// It can be either the start of a discussion (topic) or a reply to another message. 
    /// </summary>
    [DocTarget(Consts.DocsContextForum)]
    public class Post
    {
        /// <summary>
        /// The unique identifier of the forum post.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// The unique identifier of the forum.
        /// </summary>
        public Guid ForumId { get; private set; }

        /// <summary>
        /// The title of the post.
        /// It is only used if the post is the initial message (topic).
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// The slug of the post.
        /// It is only used if the post is the initial message (topic).
        /// This value is added to the URL in order to identify the topic within the forum.
        /// If the slug is "my-topic-slug", the URL will be something similar to "www.mysite.com/forum/my-forum/my-topic-slug".
        /// </summary>
        public string Slug { get; private set; }

        /// <summary>
        /// The content of the post.
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// The number of replies to the initial message.
        /// It is only used if the post is the initial message (topic).
        /// </summary>
        public int RepliesCount { get; private set; }

        /// <summary>
        /// The status of the post.
        /// It can be either published or deleted.
        /// </summary>
        public PostStatusType Status { get; private set; }

        /// <summary>
        /// The unique identifier of the user who created the post.
        /// </summary>
        [ForeignKey("CreatedByUser")]
        public Guid CreatedBy { get; private set; }

        /// <summary>
        /// The time stamp of when the post has been created.
        /// </summary>
        public DateTime CreatedOn { get; private set; }

        /// <summary>
        /// The unique identifier of the user who last updated the post.
        /// </summary>
        [ForeignKey("ModifiedByUser")]
        public Guid? ModifiedBy { get; private set; }

        /// <summary>
        /// The time stamp of when the post has been last updated.
        /// </summary>
        public DateTime? ModifiedOn { get; private set; }

        /// <summary>
        /// Value indicating whether the post is pinned or not.
        /// It is only used if the post is the initial message (topic).
        /// If pinned, the topic is always shown at the top of the forum page.
        /// </summary>
        public bool Pinned { get; private set; }

        /// <summary>
        /// Value indicating whether the post is locked or not.
        /// It is only used if the post is the initial message (topic).
        /// If locked, only users that can moderate a forum will be able to reply to the initial message.
        /// </summary>
        public bool Locked { get; private set; }

        /// <summary>
        /// Value indicating whether the post is marked as answer or not.
        /// It is only used if the post is a reply to the initial message.
        /// If it is an answer, the post is shown at the top of the topic page and marked as the accepted answer.
        /// </summary>
        public bool IsAnswer { get; private set; }

        /// <summary>
        /// Value indicating whether the post has an answer or not.
        /// It is only used if the post is the initial message (topic).
        /// If it has an answer, the topic is marked as answered in the forum page.
        /// </summary>
        public bool HasAnswer { get; private set; }

        /// <summary>
        /// The unique identifier of the topic.
        /// It is only used if the post is a reply to the initial message.
        /// </summary>
        [ForeignKey("Topic")]
        public Guid? TopicId { get; private set; }

        /// <summary>
        /// The unique identifier of the last reply.
        /// It is only used if the post is the initial message (topic).
        /// </summary>
        [ForeignKey("LastReply")]
        public Guid? LastReplyId { get; private set; }

        /// <summary>
        /// Reference to the topic.
        /// </summary>
        public virtual Post Topic { get; set; }

        /// <summary>
        /// Reference to the last reply.
        /// </summary>
        public virtual Post LastReply { get; set; }

        /// <summary>
        /// Reference to the forum.
        /// </summary>
        public virtual Forum Forum { get; set; }

        /// <summary>
        /// Reference to the user who created the post.
        /// </summary>
        public virtual User CreatedByUser { get; set; }

        /// <summary>
        /// Reference to the user who last updated the post.
        /// </summary>
        public virtual User ModifiedByUser { get; set; }

        /// <summary>
        /// Reference to post reactions.
        /// </summary>
        public virtual ICollection<PostReaction> PostReactions { get; set; }

        /// <summary>
        /// Reference to post reaction summaries.
        /// </summary>
        public virtual ICollection<PostReactionSummary> PostReactionSummaries { get; set; }

        /// <summary>
        /// Creates an empty forum post.
        /// </summary>
        public Post()
        {
        }

        /// <summary>
        /// Creates a new topic with the given values.
        /// </summary>
        /// <param name="forumId"></param>
        /// <param name="userId"></param>
        /// <param name="title"></param>
        /// <param name="slug"></param>
        /// <param name="content"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static Post CreateTopic(Guid forumId, Guid userId, string title, string slug, string content, PostStatusType status)
        {
            return new Post(Guid.NewGuid(), null, forumId, userId, title, slug, content, status);
        }

        /// <summary>
        /// Creates a new topic with the given values including the unique identifier.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="forumId"></param>
        /// <param name="userId"></param>
        /// <param name="title"></param>
        /// <param name="slug"></param>
        /// <param name="content"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static Post CreateTopic(Guid id, Guid forumId, Guid userId, string title, string slug, string content, PostStatusType status)
        {
            return new Post(id, null, forumId, userId, title, slug, content, status);
        }

        /// <summary>
        /// Creates a new reply with the given values.
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="forumId"></param>
        /// <param name="userId"></param>
        /// <param name="content"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static Post CreateReply(Guid topicId, Guid forumId, Guid userId, string content, PostStatusType status)
        {
            return new Post(Guid.NewGuid(), topicId, forumId, userId, null, null, content, status);
        }

        /// <summary>
        /// Creates a new reply with the given values including the unique identifier.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="topicId"></param>
        /// <param name="forumId"></param>
        /// <param name="userId"></param>
        /// <param name="content"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static Post CreateReply(Guid id, Guid topicId, Guid forumId, Guid userId, string content, PostStatusType status)
        {
            return new Post(id, topicId, forumId, userId, null, null, content, status);
        }

        private Post(Guid id, Guid? topicId, Guid forumId, Guid userId, string title, string slug, string content, PostStatusType status)
        {
            Id = id;
            TopicId = topicId;
            ForumId = forumId;
            CreatedBy = userId;
            Title = title;
            Slug = slug;
            Content = content;
            Status = status;
            CreatedOn = DateTime.UtcNow;
        }

        /// <summary>
        /// Updates the details of the topic.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="title"></param>
        /// <param name="slug"></param>
        /// <param name="content"></param>
        /// <param name="status"></param>
        public void UpdateDetails(Guid userId, string title, string slug, string content, PostStatusType status)
        {
            ModifiedBy = userId;
            ModifiedOn = DateTime.UtcNow;
            Title = title;
            Slug = slug;
            Content = content;
            Status = status;
        }

        /// <summary>
        /// Updates the details of the reply.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="content"></param>
        /// <param name="status"></param>
        public void UpdateDetails(Guid userId, string content, PostStatusType status)
        {
            ModifiedBy = userId;
            ModifiedOn = DateTime.UtcNow;
            Content = content;
            Status = status;
        }

        /// <summary>
        /// Updates the last reply unique identifier if the post is the initial message.
        /// </summary>
        /// <param name="lastReplyId"></param>
        public void UpdateLastReply(Guid lastReplyId)
        {
            if (IsTopic())
            {
                LastReplyId = lastReplyId;
            }
        }

        /// <summary>
        /// Increases the number of replies of the topic by 1.
        /// </summary>
        public void IncreaseRepliesCount()
        {
            RepliesCount += 1;
        }

        /// <summary>
        /// Decreases the number of replies of the topic by 1.
        /// If the resulting number is less than zero, the value is set to zero.
        /// </summary>
        public void DecreaseRepliesCount()
        {
            RepliesCount -= 1;

            if (RepliesCount < 0)
            {
                RepliesCount = 0;
            }
        }

        /// <summary>
        /// Increases count for the specified reaction type.
        /// </summary>
        /// <param name="type">The post reaction type.</param>
        public void AddReactionToSummary(PostReactionType type)
        {
            PostReactionSummaries ??= new List<PostReactionSummary>();

            var postReactionCount = PostReactionSummaries.FirstOrDefault(x => x.Type == type);
            if (postReactionCount != null)
            {
                postReactionCount.IncreaseCount();
            }
            else
            {
                PostReactionSummaries.Add(new PostReactionSummary(Id, type));
            }
        }

        /// <summary>
        /// Decreases count for the specified reaction type.
        /// </summary>
        /// <param name="type">The post reaction type.</param>
        public void RemoveReactionFromSummary(PostReactionType type)
        {
            var postReactionCount = PostReactionSummaries?.FirstOrDefault(x => x.Type == type);
            postReactionCount?.DecreaseCount();
        }

        /// <summary>
        /// Pin and unpin the topic based of the passed value.
        /// </summary>
        /// <param name="pinned"></param>
        public void Pin(bool pinned)
        {
            if (IsTopic())
            {
                Pinned = pinned;
            }
        }

        /// <summary>
        /// Lock and unlock the topic based of the passed value.
        /// </summary>
        /// <param name="locked"></param>
        public void Lock(bool locked)
        {
            if (IsTopic())
            {
                Locked = locked;
            }
        }

        /// <summary>
        /// Set the reply as answer.
        /// </summary>
        /// <param name="isAnswer"></param>
        public void SetAsAnswer(bool isAnswer)
        {
            if (!IsTopic())
            {
                IsAnswer = isAnswer;
            }
        }

        /// <summary>
        /// Set the topic as answered.
        /// </summary>
        /// <param name="hasAnswer"></param>
        public void SetAsAnswered(bool hasAnswer)
        {
            if (IsTopic())
            {
                HasAnswer = hasAnswer;
            }
        }

        /// <summary>
        /// Set the status as deleted.
        /// </summary>
        public void Delete()
        {
            Status = PostStatusType.Deleted;
        }

        /// <summary>
        /// Indicates if the post is an initial message (topic).
        /// </summary>
        /// <returns></returns>
        public bool IsTopic()
        {
            return TopicId == null;
        }
    }
}
