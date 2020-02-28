using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atlas.Models
{
    public class Post
    {
        public Guid Id { get; set; }
        public Guid ForumId { get; set; }
        public Guid? ParentId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid MemberId { get; set; }
        public int RepliesCount { get; set; }
        public Guid? LastReplyId { get; set; }

        public virtual Forum Forum { get; set; }
        public virtual Post Parent { get; set; }
        public virtual Member Member { get; set; }
        public virtual Post LastReply { get; set; }

        public virtual ICollection<Post> Replies { get; set; }

        public Post()
        {
            
        }
    }
}
