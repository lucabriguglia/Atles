using System.ComponentModel.DataAnnotations;

namespace Atles.Models.Admin.Categories;

public class FormComponentModel
{
    public CategoryModel Category { get; set; } = new();
    public IList<PermissionSetModel> PermissionSets { get; set; } = new List<PermissionSetModel>();

    public class CategoryModel
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public Guid PermissionSetId { get; set; }
    }

    public class PermissionSetModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}