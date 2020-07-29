using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Atlas.Domain.PermissionSets;

namespace Atlas.Models.Admin.PermissionSets
{
    public class FormComponentModel
    {
        public PermissionSetModel PermissionSet { get; set; } = new PermissionSetModel();

        public class PermissionSetModel
        {
            public Guid Id { get; set; }

            [Required]
            [StringLength(50)]
            public string Name { get; set; }

            public ICollection<Permission> Permissions { get; set; }
        }
    }
}