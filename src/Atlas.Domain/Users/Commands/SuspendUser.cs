using System;

namespace Atlas.Domain.Users.Commands
{
    public class SuspendUser : CommandBase
    {
        public Guid Id { get; set; }
    }
}