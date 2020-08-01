using System;
using System.Collections.Generic;
using Atlas.Domain.Categories;

namespace Atlas.Domain.Sites
{
    public class Site
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Title { get; private set; }

        public virtual ICollection<Category> Categories { get; set; }

        public Site()
        {

        }

        public Site(string name, string title)
        {
            New(Guid.NewGuid(), name, title);
        }

        public Site(Guid id, string name, string title)
        {
            New(id, name, title);
        }

        private void New(Guid id, string name, string title)
        {
            Id = id;
            Name = name;
            Title = title;
        }

        public void UpdateDetails(string title)
        {
            Title = title;
        }
    }
}
