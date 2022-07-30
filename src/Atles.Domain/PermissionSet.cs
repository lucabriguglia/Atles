using System;
using System.Collections.Generic;
using Docs.Attributes;

namespace Atles.Domain
{
    /// <summary>
    /// A permission set contains the permissions to read, create or edit forum posts.
    /// Each permission type such as view forum, read topics or create new posts is assigned to one or more user roles.
    /// </summary>
    [DocTarget(Consts.DocsContextForum)]
    public sealed class PermissionSet
    {
        /// <summary>
        /// The unique identifier of the permission set.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// The unique identifier of the site which the permission set belongs to.
        /// </summary>
        public Guid SiteId { get; private set; }

        /// <summary>
        /// The name of the permission set.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The status of the permission set.
        /// It could be either published or deleted.
        /// </summary>
        public PermissionSetStatusType Status { get; private set; }

        /// <summary>
        /// List of permissions.
        /// Each permission is a combination of a user role and a permission type.
        /// For example, role Subscriber and type Read which means that only users in the role Subscriber can read the discussions.
        /// </summary>
        public IReadOnlyCollection<Permission> Permissions => _permissions;
        private readonly List<Permission> _permissions = new();

        /// <summary>
        /// List of forum categories that use the permission set.
        /// </summary>
        public ICollection<Category> Categories { get; set; }

        /// <summary>
        /// List of forums that use the permission set.
        /// </summary>
        public ICollection<Forum> Forums { get; set; }

        /// <summary>
        /// Creates an empty permission set.
        /// </summary>
        public PermissionSet()
        {
        }

        /// <summary>
        /// Creates a new permission set  with the given values.
        /// The unique identifier is automatically assigned.
        /// The default status is published.
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="name"></param>
        /// <param name="permissions"></param>
        public PermissionSet(Guid siteId, string name, IEnumerable<Permission> permissions)
        {
            New(Guid.NewGuid(), siteId, name, permissions);
        }

        /// <summary>
        /// Creates a new permission set with the given values including a unique identifier.
        /// The default status is published.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="siteId"></param>
        /// <param name="name"></param>
        /// <param name="permissions"></param>
        public PermissionSet(Guid id, Guid siteId, string name, IEnumerable<Permission> permissions)
        {
            New(id, siteId, name, permissions);
        }

        private void New(Guid id, Guid siteId, string name, IEnumerable<Permission> permissions)
        {
            Id = id;
            SiteId = siteId;
            Name = name;
            Status = PermissionSetStatusType.Published;
            AddPermissions(permissions);
        }

        private void AddPermissions(IEnumerable<Permission> permissions)
        {
            if (permissions == null) return;

            _permissions.Clear();

            foreach (var permission in permissions)
            {
                _permissions.Add(new Permission(permission.Type, permission.RoleId));
            }
        }

        /// <summary>
        /// Updates the details of the permission set.
        /// The values that can be changed are name and permissions.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="permissions"></param>
        public void UpdateDetails(string name, ICollection<Permission> permissions)
        {
            Name = name;
            AddPermissions(permissions);
        }

        /// <summary>
        /// Set the status as deleted.
        /// The permission set can no longer be used in forum categories and forums.
        /// </summary>
        public void Delete()
        {
            Status = PermissionSetStatusType.Deleted;
        }
    }
}
