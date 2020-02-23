using ForumApp.Domain.Profiles;
using System;
using System.Collections.Generic;

namespace ForumApp.Domain.Permissions.Commands
{
    public class CreatePermissionSet
    {
        public Guid Id { get; set; }
        public Guid SiteId { get; set; }
        public string Name { get; set; }
        public List<Permission> Permissions { get; set; }
    }
}
