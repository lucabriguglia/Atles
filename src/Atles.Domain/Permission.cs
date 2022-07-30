namespace Atles.Domain
{
    public class Permission
    {
        public PermissionType Type { get; }
        public string RoleId { get; }

        public Permission()
        {
        }

        public Permission(PermissionType type, string roleId)
        {
            Type = type;
            RoleId = roleId;
        }
    }
}
