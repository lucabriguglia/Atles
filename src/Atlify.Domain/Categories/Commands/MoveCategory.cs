using System;

namespace Atlify.Domain.Categories.Commands
{
    public class MoveCategory : CommandBase
    {
        public Guid Id { get; set; }
        public Direction Direction { get; set; }
    }
}
