using Atles.Core.Queries;
using Atles.Models.Admin;
using Atles.Models.Admin.Categories;

namespace Atles.Queries.Admin
{
    public class GetCategoryForm : QueryBase<CreateCategoryFormModel>
    {
        public Guid? Id { get; set; }
    }
}
