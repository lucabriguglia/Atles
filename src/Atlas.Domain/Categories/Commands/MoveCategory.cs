using System;

namespace Atlas.Domain.Categories.Commands
{
    public class MoveCategory : CommandBase
    {
        public Guid Id { get; set; }
        public Direction Direction { get; set; }
    }
}
