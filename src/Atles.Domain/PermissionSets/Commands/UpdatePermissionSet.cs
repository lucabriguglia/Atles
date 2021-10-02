﻿using Atles.Infrastructure.Commands;
using System;
using System.Collections.Generic;
using Docs.Attributes;

namespace Atles.Domain.PermissionSets.Commands
{
    /// <summary>
    /// Request that updates a permission set.
    /// </summary>
    [DocRequest(typeof(PermissionSet))]
    public class UpdatePermissionSet : CommandBase
    {
        /// <summary>
        /// The new name for the permission set.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The new permissions for the permission set.
        /// </summary>
        public ICollection<PermissionCommand> Permissions { get; set; } = new List<PermissionCommand>();
    }
}