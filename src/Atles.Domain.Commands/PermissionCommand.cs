using Atles.Domain.Models;

namespace Atles.Domain.Commands
{
    public class PermissionCommand
    {
        public PermissionType Type { get; set; }
        public string RoleId { get; set; }
    }
}