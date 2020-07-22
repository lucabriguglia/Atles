using System;

namespace Atlas.Domain.Forums.Commands
{
    public class DeleteForum : CommandBase
    {
        public Guid ForumGroupId { get; set; }
        public Guid Id { get; set; }
    }
}
