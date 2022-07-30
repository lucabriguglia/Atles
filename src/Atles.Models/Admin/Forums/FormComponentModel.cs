using System.ComponentModel.DataAnnotations;

namespace Atles.Models.Admin.Forums
{
    public class FormComponentModel
    {
        public ForumModel Forum { get; set; } = new ForumModel();
        public IList<CategoryModel> Categories { get; set; } = new List<CategoryModel>();
        public IList<PermissionSetModel> PermissionSets { get; set; } = new List<PermissionSetModel>();

        public class ForumModel
        {
            public Guid Id { get; set; }

            [Required]
            public Guid CategoryId { get; set; }

            [Required]
            [StringLength(50)]
            public string Name { get; set; }

            [Required]
            [StringLength(50)]
            public string Slug { get; set; }

            [StringLength(200)]
            public string Description { get; set; }

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