using System;
using Atles.Infrastructure.Queries;

namespace Atles.Reporting.Models.Admin.Categories.Queries
{
    public class GetCategoryForm : QueryBase<FormComponentModel>
    {
        public Guid? Id { get; set; }
    }
}
