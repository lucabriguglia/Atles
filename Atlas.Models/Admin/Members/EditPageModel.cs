using System;
using System.ComponentModel.DataAnnotations;

namespace Atlas.Models.Admin.Members
{
    public class EditPageModel
    {
        public MemberModel Member { get; set; } = new MemberModel();

        public class MemberModel
        {
            public Guid Id { get; set; }

            [Required]
            [StringLength(50)]
            public string DisplayName { get; set; }
        }
    }
}