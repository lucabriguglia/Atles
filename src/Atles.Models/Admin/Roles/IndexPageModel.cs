using System.ComponentModel.DataAnnotations;

namespace Atles.Models.Admin.Roles
{
    public class IndexPageModel
    {
        public IList<RoleModel> Roles { get; set; } = new List<RoleModel>();

        public EditRoleModel EditRole { get; set; } = new EditRoleModel();

        public IList<UserModel> UsersInRole { get; set; } = new List<UserModel>();

        public class EditRoleModel
        {
            public string Id { get; set; }

            [Required]
            public string Name { get; set; }
        }

        public class RoleModel
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public class UserModel
        {
            public string Id { get; set; }
            public string Email { get; set; }
        }
    }
}
