using System;

namespace Atlas.Domain.Members.Commands
{
    public class CreateMember : CommandBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
    }
}
