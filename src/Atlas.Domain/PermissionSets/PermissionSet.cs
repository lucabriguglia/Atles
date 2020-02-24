using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ForumApp.Domain.Profiles
{
    public class PermissionSet
    {
        public Guid Id { get; private set; }
        public Guid SiteId { get; private set; }
        public string Name { get; private set; }

        private List<Permission> _permissions;
        public ReadOnlyCollection<Permission> Permissions => _permissions.AsReadOnly();

        public PermissionSet(Guid id, Guid siteId, string name, List<Permission> permissions)
        {
            Id = id;
            SiteId = siteId;
            Name = name;
            _permissions = permissions;
        }

        public void UpdateName(string name)
        {
            Name = name;
        }

        public void UpdatePermissions(List<Permission> permissions)
        {
            _permissions.Clear();
            _permissions = permissions;
        }
    }
}
