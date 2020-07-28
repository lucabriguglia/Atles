using System;
using System.Collections.Generic;
using Atlas.Domain.Categories;
using Atlas.Domain.Forums;

namespace Atlas.Domain.PermissionSets
{
    public sealed class PermissionSet
    {
        public Guid Id { get; private set; }
        public Guid SiteId { get; private set; }
        public string Name { get; private set; }
        public StatusType Status { get; private set; }

        public ICollection<Permission> Permissions { get; set; }
        public ICollection<Category> Categories { get; set; }
        public ICollection<Forum> Forums { get; set; }

        public PermissionSet()
        {
            
        }

        public PermissionSet(Guid siteId, string name, ICollection<Permission> permissions)
        {
            Id = Guid.NewGuid();
            SiteId = siteId;
            Name = name;
            Permissions = permissions;
            Status = StatusType.Published;
        }

        public void UpdateName(string name)
        {
            Name = name;
        }

        public void UpdatePermissions(List<Permission> permissions)
        {
            Permissions.Clear();
            Permissions = permissions;
        }

        public void Delete()
        {
            Status = StatusType.Deleted;
        }
    }
}
