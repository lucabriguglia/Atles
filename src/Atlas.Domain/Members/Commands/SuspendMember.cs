using System;

namespace Atlas.Domain.Members.Commands
{
    public class SuspendMember : CommandBase
    {
        public Guid Id { get; set; }
    }
}