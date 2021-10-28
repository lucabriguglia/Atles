using System;
using Atles.Infrastructure.Queries;

namespace Atles.Reporting.Models.Admin.PermissionSets.Queries
{
    public class GetPermissionSetEditForm : QueryBase<FormComponentModel>
    {
        public Guid Id { get; set; }
    }
}
