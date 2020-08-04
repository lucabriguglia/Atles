using System;

namespace Atlas.Domain.Categories.Commands
{
    public class DeleteCategory : CommandBase
    {
        public Guid Id { get; set; }
    }
}
