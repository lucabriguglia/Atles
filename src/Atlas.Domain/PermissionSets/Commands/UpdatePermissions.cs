using ForumApp.Domain.Profiles;
using System;
using System.Collections.Generic;

namespace ForumApp.Domain.Permissions.Commands
{
    public class UpdatePermissions
    {
        public Guid PermissionSetId { get; set; }
        public List<Permission> Permissions { get; set; }
    }
}
