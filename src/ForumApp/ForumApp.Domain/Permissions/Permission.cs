using System;
using System.Collections.Generic;

namespace ForumApp.Domain.Profiles
{
    public class Permission
    {
        public Guid Id { get; set; }
        public Guid ForumId { get; set; }
        public IList<PermissionItem> PermissionItems { get; set; }
    }
}
