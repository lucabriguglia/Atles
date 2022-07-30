using Atles.Core.Queries;
using Atles.Models.Admin.Categories;

namespace Atles.Queries.Admin
{
    public class GetCategoryForm : QueryBase<FormComponentModel>
    {
        public Guid? Id { get; set; }
    }
}
