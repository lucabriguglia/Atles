using System;

namespace Atlas.Domain.Forums.Commands
{
    public class DeleteForum : CommandBase
    {
        public Guid Id { get; set; }
    }
}
