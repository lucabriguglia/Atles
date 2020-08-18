using System;

namespace Atlify.Domain.Forums.Commands
{
    public class UpdateForum : CommandBase
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public Guid? PermissionSetId { get; set; }
    }
}
