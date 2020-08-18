using System;
using Atlify.Domain.Sites;

namespace Atlify.Domain.Pages
{
    public class Page
    {
        public Guid Id { get; private set; }
        public Guid SiteId { get; private set; }
        public string Name { get; private set; }
        public string Title { get; private set; }
        public int SortOrder { get; private set; }
        public StatusType Status { get; private set; }

        public virtual Site Site { get; set; }
    }
}
