using System;

namespace Atlas.Domain.ForumGroups.Commands
{
    public class MoveForumGroup : CommandBase
    {
        public Guid Id { get; set; }
        public Direction Direction { get; set; }
    }
}
