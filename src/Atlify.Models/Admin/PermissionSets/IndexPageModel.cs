using System;
using System.Collections.Generic;

namespace Atlify.Models.Admin.PermissionSets
{
    public class IndexPageModel
    {
        public IList<PermissionSetModel> PermissionSets { get; set; } = new List<PermissionSetModel>();

        public class PermissionSetModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public bool IsInUse { get; set; }
        }
    }
}
