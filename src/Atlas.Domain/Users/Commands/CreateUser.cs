using System;

namespace Atlas.Domain.Users.Commands
{
    public class CreateUser : CommandBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string IdentityUserId { get; set; }
        public string Email { get; set; }
        public bool Confirm { get; set; }
    }
}
