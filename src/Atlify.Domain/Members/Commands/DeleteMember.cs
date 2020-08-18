using System;

namespace Atlify.Domain.Members.Commands
{
    public class DeleteMember : CommandBase
    {
        public Guid Id { get; set; }
    }
}
