using System;
using System.Collections.Generic;

namespace Atlas.Domain.Members.Commands
{
    public class UpdateMember : CommandBase
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public IList<string> Roles { get; set; } = null;
    }
}
