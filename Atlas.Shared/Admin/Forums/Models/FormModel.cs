using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Atlas.Shared.Admin.Forums.Models
{
    public class FormModel
    {
        public ForumModel Forum { get; set; } = new ForumModel();
        public IList<CategoryModel> Categories { get; set; } = new List<CategoryModel>();
        public IList<PermissionSetModel> PermissionSets { get; set; } = new List<PermissionSetModel>();

        public class ForumModel
        {
            public Guid Id { get; set; }

            public Guid CategoryId { get; set; }

            [Required]
            [StringLength(100)]
            public string Name { get; set; }

            public Guid PermissionSetId { get; set; } = Guid.Empty;
        }

        public class CategoryModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class PermissionSetModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }
    }
}