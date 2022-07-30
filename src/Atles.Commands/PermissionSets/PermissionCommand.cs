using Atles.Domain;

namespace Atles.Commands.PermissionSets
{
    public class PermissionCommand
    {
        public PermissionType Type { get; set; }
        public string RoleId { get; set; }
    }
}