using System;

namespace Atlas.Server.Domain.Commands
{
    public class CreateForumGroup
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? PermissionSetId { get; set; }
    }
}
