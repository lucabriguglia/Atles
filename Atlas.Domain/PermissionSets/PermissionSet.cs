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
            New(Guid.NewGuid(), siteId, name, permissions);
        }

        public PermissionSet(Guid id, Guid siteId, string name, ICollection<Permission> permissions)
        {
            New(id, siteId, name, permissions);
        }

        public void New(Guid id, Guid siteId, string name, ICollection<Permission> permissions)
        {
            Id = id;
            SiteId = siteId;
            Name = name;
            Permissions = permissions;
            Status = StatusType.Published;
        }

        public void UpdateDetails(string name, ICollection<Permission> permissions)
        {
            Name = name;
            Permissions.Clear();
            Permissions = permissions;
        }

        public void Delete()
        {
            Status = StatusType.Deleted;
        }
    }
}
