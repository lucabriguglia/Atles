using System;
using System.ComponentModel.DataAnnotations;

namespace Atlas.Shared.Models.Admin.ForumGroups
{
    public class EditModel
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public Guid? PermissionSetId { get; set; }
    }
}