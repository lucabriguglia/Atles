using System;
using Docs.Attributes;

namespace Atlas.Domain.Forums.Commands
{
    [DocRequest(typeof(Forum))]
    public class CreateForum : CommandBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public Guid? PermissionSetId { get; set; }
    }
}
