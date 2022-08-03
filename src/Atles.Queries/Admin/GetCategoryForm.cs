using Atles.Core.Queries;
using Atles.Models.Admin;

namespace Atles.Queries.Admin
{
    public class GetCategoryForm : QueryBase<CategoryFormModel>
    {
        public Guid? Id { get; set; }
    }
}
