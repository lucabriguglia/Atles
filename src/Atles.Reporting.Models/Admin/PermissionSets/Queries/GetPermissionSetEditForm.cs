using Atles.Infrastructure.Queries;
using System;

namespace Atles.Models.Admin.PermissionSets.Queries
{
    public class GetPermissionSetEditForm : QueryBase<FormComponentModel>
    {
        public Guid Id { get; set; }
    }
}
