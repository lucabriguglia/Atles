using System;

namespace Atlas.Domain.Members.Commands
{
    public class UpdateMember : CommandBase
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
    }
}
