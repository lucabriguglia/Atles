using Atles.Core.Queries;
using Atles.Models.Public;

namespace Atles.Queries.Public
{
    public class GetPermissions : QueryBase<IList<PermissionModel>>
    {
        public Guid? PermissionSetId { get; set; }
        public Guid? ForumId { get; set; }
    }
}
