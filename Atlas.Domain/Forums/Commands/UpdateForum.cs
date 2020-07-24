using System;

namespace Atlas.Domain.Forums.Commands
{
    public class UpdateForum : CommandBase
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public Guid? PermissionSetId { get; set; }
    }
}
