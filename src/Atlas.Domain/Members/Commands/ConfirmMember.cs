using System;

namespace Atlas.Domain.Members.Commands
{
    public class ConfirmMember : CommandBase
    {
        public Guid Id { get; set; }
    }
}