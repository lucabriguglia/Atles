using Atles.Core.Queries;
using Atles.Models.Admin;

namespace Atles.Queries.Admin
{
    public class GetCategoryForm : QueryBase<CreateCategoryFormModel>
    {
        public Guid? Id { get; set; }
    }
}
