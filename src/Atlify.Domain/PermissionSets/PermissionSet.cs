using System;
using System.Collections.Generic;
using Atlify.Domain.Categories;
using Atlify.Domain.Forums;
using Atlify.Domain.PermissionSets.Commands;

namespace Atlify.Domain.PermissionSets
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

        public PermissionSet(Guid siteId, string name, ICollection<PermissionCommand> permissions)
        {
            New(Guid.NewGuid(), siteId, name, permissions);
        }

        public PermissionSet(Guid id, Guid siteId, string name, ICollection<PermissionCommand> permissions)
        {
            New(id, siteId, name, permissions);
        }

        public void New(Guid id, Guid siteId, string name, ICollection<PermissionCommand> permissions)
        {
            Id = id;
            SiteId = siteId;
            Name = name;
            Status = StatusType.Published;
            AddPermissions(permissions);
        }

        private void AddPermissions(ICollection<PermissionCommand> permissions)
        {
            Permissions = new List<Permission>();

            foreach (var permission in permissions)
            {
                Permissions.Add(new Permission(Id, permission.Type, permission.RoleId));
            }
        }

        public void UpdateDetails(string name, ICollection<PermissionCommand> permissions)
        {
            Name = name;
            AddPermissions(permissions);
        }

        public void Delete()
        {
            Status = StatusType.Deleted;
        }
    }
}
