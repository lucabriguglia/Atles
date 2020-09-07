using System;
using System.Collections.Generic;
using Atlas.Domain.Forums;
using Atlas.Domain.PermissionSets;
using Atlas.Domain.Sites;
using Docs.Attributes;

namespace Atlas.Domain.Categories
{
    /// <summary>
    /// Forum category.
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
        /// The number of topics of all forums within the Forum Category.
        /// </summary>
        public int TopicsCount { get; private set; }

        /// <summary>
        /// The number of replies of all forums within the Forum Category.
        /// </summary>
        public int RepliesCount { get; private set; }

        /// <summary>
        /// The status of the Forum Category.
        /// Can be either Published or Deleted.
        /// </summary>
        public StatusType Status { get; private set; }

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
        /// Create an empty Forum Category.
        /// Only used by Entity Framework.
        /// </summary>
        public Category()
        {

        }

        /// <summary>
        /// Create a new Forum Category with the given values including a unique identifier.
        /// The default Status is Published.
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
        /// Create a new Forum Category with the given values.
        /// The unique identifier is automatically assigned.
        /// The default Status is Published.
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
            Status = StatusType.Published;
        }

        /// <summary>
        /// Updates the details of the Forum Category.
        /// The values that can be changed are the name and the permission set.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="permissionSetId"></param>
        public void UpdateDetails(string name, Guid permissionSetId)
        {
            Name = name;
            PermissionSetId = permissionSetId;
        }

        /// <summary>
        /// Change the Sort Order by moving up 1 position.
        /// It generates an error if Sort Order is 1.
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
        /// Change the Sort Order by moving down 1 position.
        /// </summary>
        public void MoveDown()
        {
            SortOrder += 1;
        }

        /// <summary>
        /// Replace the Sort Order with the given value.
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
        /// Set the status as Deleted and Forums, Topics and Replies within the Forum Category will no longer be visible.
        /// </summary>
        public void Delete()
        {
            Status = StatusType.Deleted;
        }

        /// <summary>
        /// Returns the name of Permission Set used for the Forum Category.
        /// </summary>
        /// <returns></returns>
        public string PermissionSetName() => PermissionSet?.Name;
    }    
}
