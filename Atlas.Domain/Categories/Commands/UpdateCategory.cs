using System;

namespace Atlas.Domain.Categories.Commands
{
    public class UpdateCategory : CommandBase
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? PermissionSetId { get; set; }
    }
}
