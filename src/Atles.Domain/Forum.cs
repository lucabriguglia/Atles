using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Docs.Attributes;

namespace Atles.Domain
{
    /// <summary>
    /// A forum is a board where users can hold conversations.
    /// </summary>
    [DocTarget(Consts.DocsContextForum)]
    public class Forum
    {
        /// <summary>
        /// The unique identifier of the Forum.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// The unique identifier of the Forum Category which the Forum belongs to.
        /// </summary>
        public Guid CategoryId { get; private set; }

        /// <summary>
        /// The name of the Forum.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The Slug of the Forum.
        /// This value is added to the URL in order to identify the Forum.
        /// If the Slug is "my-forum-slug", the URL will be something similar to "www.mysite.com/forum/my-forum-slug".
        /// </summary>
        public string Slug { get; private set; }

        /// <summary>
        /// The description of the Forum.
        /// This value will be displayed in the index and forum pages if included in the theme.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// The sort order which is used to display the forums in the index page.  
        /// </summary>
        public int SortOrder { get; private set; }

        /// <summary>
        /// The number of topics published.
        /// </summary>
        public int TopicsCount { get; private set; }

        /// <summary>
        /// The number of replies published.
        /// </summary>
        public int RepliesCount { get; private set; }

        /// <summary>
        /// The status of the forum.
        /// Can be either published or deleted.
        /// </summary>
        public ForumStatusType Status { get; private set; }

        /// <summary>
        /// The unique identifier of the permission set used.
        /// If it's not assigned, the forum will inherit the permission set fom the forum category.
        /// </summary>
        public Guid? PermissionSetId { get; private set; }

        /// <summary>
        /// The unique identifier of the last post published.
        /// </summary>
        [ForeignKey("LastPost")]
        public Guid? LastPostId { get; private set; }

        /// <summary>
        /// Reference to the forum category.
        /// </summary>
        public virtual Category Category { get; set; }

        /// <summary>
        /// Reference to the permission set if assigned.
        /// </summary>
        public virtual PermissionSet PermissionSet { get; set; }

        /// <summary>
        /// Reference to the last post published if any.
        /// </summary>
        public virtual Post LastPost { get; set; }

        /// <summary>
        /// List of published posts.
        /// </summary>
        public virtual ICollection<Post> Posts { get; set; }

        /// <summary>
        /// Create an empty forum.
        /// </summary>
        public Forum()
        {
        }

        /// <summary>
        /// Create a new forum with the given values including a unique identifier.
        /// The default status is published.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="categoryId"></param>
        /// <param name="name"></param>
        /// <param name="slug"></param>
        /// <param name="description"></param>
        /// <param name="sortOrder"></param>
        /// <param name="permissionSetId"></param>
        public Forum(Guid id, Guid categoryId, string name, string slug, string description, int sortOrder, Guid? permissionSetId = null)
        {
            New(id, categoryId, name, slug, description, sortOrder, permissionSetId);
        }

        /// <summary>
        /// Creates a new forum with the given values.
        /// The unique identifier is automatically assigned.
        /// The default status is published.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="name"></param>
        /// <param name="slug"></param>
        /// <param name="description"></param>
        /// <param name="sortOrder"></param>
        /// <param name="permissionSetId"></param>
        public Forum(Guid categoryId, string name, string slug, string description, int sortOrder, Guid? permissionSetId = null)
        {
            New(Guid.NewGuid(), categoryId, name, slug, description, sortOrder, permissionSetId);
        }

        private void New(Guid id, Guid categoryId, string name, string slug, string description, int sortOrder, Guid? permissionSetId = null)
        {
            Id = id;
            CategoryId = categoryId;
            Name = name;
            Slug = slug;
            Description = description;
            SortOrder = sortOrder;
            PermissionSetId = permissionSetId;
            Status = ForumStatusType.Published;
        }

        /// <summary>
        /// Updates the details of the forum.
        /// The values that can be changed are forum category, name, slug, description and permission set.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="name"></param>
        /// <param name="slug"></param>
        /// <param name="description"></param>
        /// <param name="permissionSetId"></param>
        public void UpdateDetails(Guid categoryId, string name, string slug, string description, Guid? permissionSetId = null)
        {
            CategoryId = categoryId;
            Name = name;
            Slug = slug;
            Description = description;
            PermissionSetId = permissionSetId;
        }

        /// <summary>
        /// Updates the unique identifier of the last published post.
        /// </summary>
        /// <param name="lastPostId"></param>
        public void UpdateLastPost(Guid? lastPostId)
        {
            LastPostId = lastPostId;
        }

        /// <summary>
        /// Change the sort order by moving up 1 position.
        /// It generates an error if sort order is 1.
        /// </summary>
        /// <exception cref="ApplicationException"></exception>
        public void MoveUp()
        {
            if (SortOrder == 1)
            {
                throw new ApplicationException($"Forum \"{Name}\" can't be moved up.");
            }

            SortOrder -= 1;
        }

        /// <summary>
        /// Change the sort order by moving down 1 position.
        /// </summary>
        public void MoveDown()
        {
            SortOrder += 1;
        }

        /// <summary>
        /// Replace the sort order with the given value.
        /// </summary>
        /// <param name="sortOrder"></param>
        public void Reorder(int sortOrder)
        {
            SortOrder = sortOrder;
        }

        /// <summary>
        /// Increase the number of topics by 1.
        /// </summary>
        public void IncreaseTopicsCount()
        {
            TopicsCount += 1;
        }

        /// <summary>
        /// Decrease the number of topics by the given value.
        /// If no value is given it will decrease by 1.
        /// If the resulting number is less than zero, the value will be set to zero.
        /// </summary>
        /// <param name="count"></param>
        public void DecreaseTopicsCount(int count = 1)
        {
            TopicsCount -= count;

            if (TopicsCount < 0)
            {
                TopicsCount = 0;
            }
        }

        /// <summary>
        /// Increase the number of replies by 1.
        /// </summary>
        public void IncreaseRepliesCount()
        {
            RepliesCount += 1;
        }

        /// <summary>
        /// Decrease the number of replies by the given value.
        /// If no value is given it will decrease by 1.
        /// If the resulting number is less than zero, the value will be set to zero.
        /// </summary>
        /// <param name="count"></param>
        public void DecreaseRepliesCount(int count = 1)
        {
            RepliesCount -= count;

            if (RepliesCount < 0)
            {
                RepliesCount = 0;
            }
        }

        /// <summary>
        /// Set the status as deleted.
        /// Topics and replies within the forum will no longer be visible.
        /// </summary>
        public void Delete()
        {
            Status = ForumStatusType.Deleted;
        }

        /// <summary>
        /// Returns the name of permission set if assigned.
        /// </summary>
        /// <returns></returns>
        public string PermissionSetName() => PermissionSet?.Name;

        /// <summary>
        /// Indicates if a permission set is assigned.
        /// </summary>
        /// <returns></returns>
        public bool HasPermissionSet() => PermissionSet != null;
    }
}
