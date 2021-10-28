namespace Atles.Domain.Models.PermissionSets.Commands
{
    public class PermissionCommand
    {
        public PermissionType Type { get; set; }
        public string RoleId { get; set; }
    }
}