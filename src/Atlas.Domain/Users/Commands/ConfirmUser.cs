using System;

namespace Atlas.Domain.Users.Commands
{
    public class ConfirmUser : CommandBase
    {
        public Guid Id { get; set; }
    }
}