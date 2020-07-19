using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Atlas.Shared.Models.Admin.ForumGroups
{
    public class CreateModel
    {
        public ForumGroupModel ForumGroup { get; set; } = new ForumGroupModel();
        public IList<PermissionSetModel> PermissionSets { get; set; } = new List<PermissionSetModel>();

        public class ForumGroupModel
        {
            [Required]
            [StringLength(100)]
            public string Name { get; set; }

            public Guid? PermissionSetId { get; set; }
        }

        public class PermissionSetModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }
    }
}