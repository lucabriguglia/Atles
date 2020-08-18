using System;

namespace Atlify.Domain.Members.Commands
{
    public class SuspendMember : CommandBase
    {
        public Guid Id { get; set; }
    }
}