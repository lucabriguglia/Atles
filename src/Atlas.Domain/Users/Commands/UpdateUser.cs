using System;
using System.Collections.Generic;

namespace Atlas.Domain.Users.Commands
{
    public class UpdateUser : CommandBase
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public IList<string> Roles { get; set; } = null;
    }
}
