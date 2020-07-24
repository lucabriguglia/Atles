using System;
using System.ComponentModel.DataAnnotations;

namespace Atlas.Shared.Public.Models
{
    public class TopicPageModel
    {
        public ForumModel Forum { get; set; } = new ForumModel();
        public TopicModel Topic { get; set; }

        public class ForumModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class TopicModel
        {
            public Guid Id { get; set; }

            [Required]
            [StringLength(100)]
            public string Title { get; set; }

            [Required]
            public string Content { get; set; }
        }
    }
}
