using Atles.Core.Queries;
using Atles.Models.Admin.PermissionSets;

namespace Atles.Queries.Admin
{
    public class GetPermissionSetEditForm : QueryBase<FormComponentModel>
    {
        public Guid Id { get; set; }
    }
}
