using System;

namespace Atlify.Domain.Categories.Commands
{
    public class DeleteCategory : CommandBase
    {
        public Guid Id { get; set; }
    }
}
