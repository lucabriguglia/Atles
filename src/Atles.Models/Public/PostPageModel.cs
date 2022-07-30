using System.ComponentModel.DataAnnotations;

namespace Atles.Models.Public
{
    public class PostPageModel
    {
        public ForumModel Forum { get; set; } = new ForumModel();

        public TopicModel Topic { get; set; } = new TopicModel();

        public class ForumModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Slug { get; set; }
        }

        public class TopicModel
        {
            public Guid Id { get; set; }

            [Required]
            [StringLength(100)]
            public string Title { get; set; }

            public string Slug { get; set; }

            [Required]
            public string Content { get; set; }

            public Guid UserId { get; set; }

            public bool Locked { get; set; }

            public bool Subscribe { get; set; }
        }
    }
}
