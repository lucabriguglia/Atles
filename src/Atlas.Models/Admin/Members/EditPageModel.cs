using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Atlas.Domain;

namespace Atlas.Models.Admin.Members
{
    public class EditPageModel
    {
        public MemberModel Member { get; set; } = new MemberModel();
        public InfoModel Info { get; set; } = new InfoModel();
        public IList<RoleModel> Roles { get; set; } = new List<RoleModel>();

        public class MemberModel
        {
            public Guid Id { get; set; }

            [Required]
            [StringLength(50)]
            public string DisplayName { get; set; }
        }

        public class InfoModel
        {
            public StatusType Status { get; set; }
            public string UserId { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public bool EmailConfirmed { get; set; }
            public string PhoneNumber { get; set; }
            public bool PhoneNumberConfirmed { get; set; }
            public bool TwoFactorEnabled { get; set; }
            public bool LockoutEnabled { get; set; }
            public int AccessFailedCount { get; set; }
            public DateTimeOffset? LockoutEnd { get; set; }
        }

        public class RoleModel
        {
            public string Name { get; set; }
            public bool Selected { get; set; }
        }
    }
}