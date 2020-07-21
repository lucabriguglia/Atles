using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Atlas.Shared.Models.Admin.ForumGroups
{
    public class FormModel
    {
        public ForumGroupModel ForumGroup { get; set; } = new ForumGroupModel();
        public IList<PermissionSetModel> PermissionSets { get; set; } = new List<PermissionSetModel>();

        public class ForumGroupModel
        {
            public Guid Id { get; set; }

            [Required]
            [StringLength(100)]
            public string Name { get; set; }

            public Guid PermissionSetId { get; set; } = Guid.Empty;
        }

        public class PermissionSetModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }
    }
}