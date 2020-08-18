using System;

namespace Atlify.Domain.Categories.Commands
{
    public class CreateCategory : CommandBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public Guid PermissionSetId { get; set; }
    }
}
