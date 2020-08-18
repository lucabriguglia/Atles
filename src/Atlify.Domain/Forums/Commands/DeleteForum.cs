using System;

namespace Atlify.Domain.Forums.Commands
{
    public class DeleteForum : CommandBase
    {
        public Guid Id { get; set; }
    }
}
