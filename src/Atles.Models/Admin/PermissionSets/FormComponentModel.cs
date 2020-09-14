using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Atles.Domain.PermissionSets;

namespace Atles.Models.Admin.PermissionSets
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

            public IList<PermissionModel> Permissions { get; set; } = new List<PermissionModel>();
        }

        public class PermissionModel
        {
            public string RoleId { get; set; }
            public string RoleName { get; set; }
            //public bool Disabled { get; set; }

            public IList<PermissionTypeModel> PermissionTypes { get; set; } = new List<PermissionTypeModel>();
        }

        public class PermissionTypeModel
        {
            public PermissionType Type { get; set; }
            public bool Selected { get; set; }
            public bool Disabled { get; set; }
        }
    }
}