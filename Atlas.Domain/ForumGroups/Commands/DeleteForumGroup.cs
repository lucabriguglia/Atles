using System;

namespace Atlas.Domain.ForumGroups.Commands
{
    public class DeleteForumGroup : CommandBase
    {
        public Guid Id { get; set; }
    }
}
