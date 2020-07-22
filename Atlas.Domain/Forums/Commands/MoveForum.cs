using System;

namespace Atlas.Domain.Forums.Commands
{
    public class MoveForum : CommandBase
    {
        public Guid ForumGroupId { get; set; }
        public Guid Id { get; set; }
        public Direction Direction { get; set; }
    }
}
