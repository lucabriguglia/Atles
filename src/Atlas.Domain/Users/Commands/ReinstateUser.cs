using System;

namespace Atlas.Domain.Users.Commands
{
    public class ReinstateUser : CommandBase
    {
        public Guid Id { get; set; }
    }
}