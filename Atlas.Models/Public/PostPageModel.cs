using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Atlas.Models.Public
{
    public class PostPageModel
    {
        public ForumModel Forum { get; set; } = new ForumModel();

        public IList<PermissionModel> Permissions { get; set; } = new List<PermissionModel>();
        public TopicModel Topic { get; set; } = new TopicModel();

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
