using System;
using Atles.Core.Queries;

namespace Atles.Reporting.Models.Admin.PermissionSets.Queries
{
    public class GetPermissionSetEditForm : QueryBase<FormComponentModel>
    {
        public Guid Id { get; set; }
    }
}
