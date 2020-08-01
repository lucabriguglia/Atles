using System;

namespace Atlas.Domain.Members.Commands
{
    public class DeleteMember : CommandBase
    {
        public Guid Id { get; set; }
    }
}
