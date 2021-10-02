using Atles.Infrastructure.Queries;
using Atles.Models.Admin.Categories;
using System;

namespace Atles.Reporting.Admin.Categories
{
    public class GetCategoryForm : QueryBase<FormComponentModel>
    {
        public Guid? Id { get; set; }
    }
}
