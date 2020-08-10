using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Atlas.Domain.Categories;
using Atlas.Domain.PermissionSets;
using Atlas.Domain.Posts;

namespace Atlas.Domain.Forums
{
    public class Forum
    {
        public Guid Id { get; private set; }
        public Guid CategoryId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int SortOrder { get; private set; }
        public int TopicsCount { get; private set; }
        public int RepliesCount { get; private set; }
        public StatusType Status { get; private set; }
        public Guid? PermissionSetId { get; private set; }

        [ForeignKey("LastPost")]
        public Guid? LastPostId { get; private set; }

        public virtual Category Category { get; set; }
        public virtual PermissionSet PermissionSet { get; set; }
        public virtual Post LastPost { get; set; }

        public virtual ICollection<Post> Posts { get; set; }

        public Forum()
        {
            
        }

        public Forum(Guid id, Guid categoryId, string name, string description, int sortOrder, Guid? permissionSetId = null)
        {
            New(id, categoryId, name, description, sortOrder, permissionSetId);
        }

        public Forum(Guid categoryId, string name, string description, int sortOrder, Guid? permissionSetId = null)
        {
            New(Guid.NewGuid(), categoryId, name, description, sortOrder, permissionSetId);
        }

        private void New(Guid id, Guid categoryId, string name, string description, int sortOrder, Guid? permissionSetId = null)
        {
            Id = id;
            CategoryId = categoryId;
            Name = name;
            Description = description;
            SortOrder = sortOrder;
            PermissionSetId = permissionSetId;
            Status = StatusType.Published;
        }

        public void UpdateDetails(Guid categoryId, string name, string description, Guid? permissionSetId = null)
        {
            CategoryId = categoryId;
            Name = name;
            Description = description;
            PermissionSetId = permissionSetId;
        }

        public void UpdateLastPost(Guid? lastPostId)
        {
            LastPostId = lastPostId;
        }
        public void MoveUp()
        {
            if (SortOrder == 1)
            {
                throw new ApplicationException($"Forum \"{Name}\" can't be moved up.");
            }

            SortOrder -= 1;
        }

        public void MoveDown()
        {
            SortOrder += 1;
        }

        public void Reorder(int sortOrder)
        {
            SortOrder = sortOrder;
        }

        public void IncreaseTopicsCount()
        {
            TopicsCount += 1;
        }

        public void DecreaseTopicsCount()
        {
            TopicsCount -= 1;

            if (TopicsCount < 0)
            {
                TopicsCount = 0;
            }
        }

        public void IncreaseRepliesCount()
        {
            RepliesCount += 1;
        }

        public void DecreaseRepliesCount()
        {
            RepliesCount -= 1;

            if (RepliesCount < 0)
            {
                RepliesCount = 0;
            }
        }

        public void Delete()
        {
            Status = StatusType.Deleted;
        }

        public string PermissionSetName() => PermissionSet?.Name;

        public bool HasPermissionSet() => PermissionSet != null;
    }
}
