using System;

namespace Atlify.Domain.Members.Commands
{
    public class ReinstateMember : CommandBase
    {
        public Guid Id { get; set; }
    }
}