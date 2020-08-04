using System;

namespace Atlas.Domain
{
    public abstract class CommandBase
    {
        public Guid SiteId { get; set; }
        public Guid MemberId { get; set; }
    }
}
