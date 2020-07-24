using System;
using System.Collections.Generic;

namespace Atlas.Shared.Public.Models
{
    public class ForumPageModel
    {
        public ForumModel Forum { get; set; }
        public IList<TopicModel> Topics { get; set; } = new List<TopicModel>();

        public class ForumModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class TopicModel
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
        }
    }
}
