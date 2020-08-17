using System;

namespace Atlas.Domain.Members.Commands
{
    public class ReinstateMember : CommandBase
    {
        public Guid Id { get; set; }
    }
}