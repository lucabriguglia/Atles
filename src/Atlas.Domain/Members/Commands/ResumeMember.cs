using System;

namespace Atlas.Domain.Members.Commands
{
    public class ResumeMember : CommandBase
    {
        public Guid Id { get; set; }
    }
}