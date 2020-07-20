using System;
using System.Collections.Generic;

namespace Atlas.Domain
{
    public class Forum
    {
        public Guid Id { get; private set; }
        public Guid ForumGroupId { get; private set; }
        public string Name { get; private set; }
        public int SortOrder { get; private set; }
        public int TopicsCount { get; private set; }
        public int RepliesCount { get; private set; }
        public StatusType Status { get; private set; }
        public Guid? PermissionSetId { get; private set; }

        public virtual ForumGroup ForumGroup { get; set; }
        public virtual PermissionSet PermissionSet { get; set; }

        public virtual ICollection<Topic> Topics { get; set; }

        public Forum()
        {
            
        }

        public Forum(Guid forumGroupId, string name, int sortOrder, Guid? permissionSetId = null)
        {
            Id = Guid.NewGuid();
            ForumGroupId = forumGroupId;
            Name = name;
            SortOrder = sortOrder;
            PermissionSetId = permissionSetId;
            Status = StatusType.Published;
        }

        public void UpdateDetails(Guid forumGroupId, string name, Guid? permissionSetId = null)
        {
            ForumGroupId = forumGroupId;
            Name = name;
            PermissionSetId = permissionSetId;
        }

        public void UpdateOrder(int sortOrder)
        {
            SortOrder = sortOrder;
        }

        public void IncreaseTopicsCount()
        {
            TopicsCount += 1;
        }

        public void IncreaseRepliesCount()
        {
            RepliesCount += 1;
        }

        public void DecreaseTopicsCount()
        {
            TopicsCount -= 1;
            if (TopicsCount < 0) TopicsCount = 0;
        }

        public void DecreaseRepliesCount()
        {
            RepliesCount -= 1;
            if (RepliesCount < 0) RepliesCount = 0;
        }

        public void Delete()
        {
            Status = StatusType.Deleted;
        }

        public string PermissionSetName() => PermissionSet?.Name;

        public bool HasPermissionSet() => PermissionSet != null;
    }
}
