using System;

namespace Atlas.Models
{
    public class ForumGroup
    {
        public Guid Id { get; set; }
        public Guid SiteId { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public int Topics { get; set; }
        public int Replies { get; set; }
        public Guid? PermissionSetId { get; set; }
        public Guid? LastTopicId { get; set; }
        public Guid? LastReplyId { get; set; }

        public virtual Site Site { get; set; }
        public virtual Topic LastTopic { get; set; }
        public virtual Reply LastReply { get; set; }
    }    
}
