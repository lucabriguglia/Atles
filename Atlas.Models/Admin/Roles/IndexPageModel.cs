using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Atlas.Models.Admin.Roles
{
    public class IndexPageModel
    {
        public IList<RoleModel> Roles { get; set; } = new List<RoleModel>();

        public EditRoleModel EditRole { get; set; } = new EditRoleModel();

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
    }
}
