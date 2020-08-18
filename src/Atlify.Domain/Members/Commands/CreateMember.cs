using System;

namespace Atlify.Domain.Members.Commands
{
    public class CreateMember : CommandBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }
        public string Email { get; set; }
    }
}
