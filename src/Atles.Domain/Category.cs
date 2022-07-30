using System;
using System.Collections.Generic;
using Docs.Attributes;

namespace Atles.Domain
{
    /// <summary>
    /// A forum category acts as a container to group logically different forums.
    /// </summary>
    [DocTarget(Consts.DocsContextForum)]
    public class Category
    {
        /// <summary>
        /// The unique identifier of the Forum Category.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// The unique identifier of the Site which the Forum Category belongs to.
        /// </summary>
        public Guid SiteId { get; private set; }

        /// <summary>
        /// The name of the Forum Category which is displayed in the index page.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The sort order which is used to display the Forum Categories in the index page.  
        /// </summary>
        public int SortOrder { get; private set; }

        /// <summary>
        /// The number of topics of all forums within the forum category.
        /// </summary>
        public int TopicsCount { get; private set; }

        /// <summary>
        /// The number of replies of all forums within the forum category.
        /// </summary>
        public int RepliesCount { get; private set; }

        /// <summary>
        /// The status of the forum category.
        /// Can be either published or deleted.
        /// </summary>
        public CategoryStatusType Status { get; private set; }

        /// <summary>
        /// The unique identifier of the Permission Set used as default for all Forums within the Forum Category which do not specify a Permission Set.
        /// </summary>
        public Guid PermissionSetId { get; private set; }

        /// <summary>
        /// Reference to the Site the Forum Category belongs to.
        /// </summary>
        public virtual Site Site { get; set; }

        /// <summary>
        /// Reference to Permission Set used for the Forum Category.
        /// </summary>
        public virtual PermissionSet PermissionSet { get; set; }

        /// <summary>
        /// List of Forums that belong to the Forum Category.
        /// </summary>
        public virtual ICollection<Forum> Forums { get; set; }

        /// <summary>
        /// Creates an empty forum category.
        /// </summary>
        public Category()
        {
        }

        /// <summary>
        /// Creates a new forum category with the given values including a unique identifier.
        /// The default status is published.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="siteId"></param>
        /// <param name="name"></param>
        /// <param name="sortOrder"></param>
        /// <param name="permissionSetId"></param>
        public Category(Guid id, Guid siteId, string name, int sortOrder, Guid permissionSetId)
        {
            New(id, siteId, name, sortOrder, permissionSetId);
        }

        /// <summary>
        /// Creates a new forum category with the given values.
        /// The unique identifier is automatically assigned.
        /// The default status is published.
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="name"></param>
        /// <param name="sortOrder"></param>
        /// <param name="permissionSetId"></param>
        public Category(Guid siteId, string name, int sortOrder, Guid permissionSetId)
        {
            New(Guid.NewGuid(), siteId, name, sortOrder, permissionSetId);
        }

        private void New(Guid id, Guid siteId, string name, int sortOrder, Guid permissionSetId)
        {
            Id = id;
            SiteId = siteId;
            Name = name;
            SortOrder = sortOrder;
            PermissionSetId = permissionSetId;
            Status = CategoryStatusType.Published;
        }

        /// <summary>
        /// Updates the details of the forum category.
        /// The values that can be changed are name and permission set.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="permissionSetId"></param>
        public void UpdateDetails(string name, Guid permissionSetId)
        {
            Name = name;
            PermissionSetId = permissionSetId;
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
                throw new ApplicationException($"Category \"{Name}\" can't be moved up.");
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
        /// Increase the number of topics by the given value.
        /// If no value is given it will increase by 1.
        /// </summary>
        /// <param name="count"></param>
        public void IncreaseTopicsCount(int count = 1)
        {
            TopicsCount += count;
        }

        /// <summary>
        /// Increase the number of replies by the given value.
        /// If no value is given it will increase by 1.
        /// </summary>
        /// <param name="count"></param>
        public void IncreaseRepliesCount(int count = 1)
        {
            RepliesCount += count;
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
        /// Forums, topics and replies within the forum category will no longer be visible.
        /// </summary>
        public void Delete()
        {
            Status = CategoryStatusType.Deleted;
        }

        /// <summary>
        /// Returns the name of permission set used.
        /// </summary>
        /// <returns></returns>
        public string PermissionSetName() => PermissionSet?.Name;
    }    
}
