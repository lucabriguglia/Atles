using System;
using System.Collections.Generic;
using Docs.Attributes;

namespace Atlas.Domain.Users.Commands
{
    [DocRequest(typeof(User))]
    public class UpdateUser : CommandBase
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public IList<string> Roles { get; set; } = null;
    }
}
