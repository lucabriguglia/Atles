using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Atlas.Models.Admin.Members
{
    public class EditPageModel
    {
        public MemberModel Member { get; set; } = new MemberModel();
        public IList<RoleModel> Roles { get; set; } = new List<RoleModel>();

        public class MemberModel
        {
            public Guid Id { get; set; }

            [Required]
            [StringLength(50)]
            public string DisplayName { get; set; }

            public string UserId { get; set; }
        }

        public class RoleModel
        {
            public string Name { get; set; }
            public bool Selected { get; set; }
        }
    }
}