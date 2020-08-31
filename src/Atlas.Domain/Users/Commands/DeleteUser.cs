using System;

namespace Atlas.Domain.Users.Commands
{
    public class DeleteUser : CommandBase
    {
        public Guid Id { get; set; }
    }
}
