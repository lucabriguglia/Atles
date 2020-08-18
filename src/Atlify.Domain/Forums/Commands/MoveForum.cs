using System;

namespace Atlify.Domain.Forums.Commands
{
    public class MoveForum : CommandBase
    {
        public Guid Id { get; set; }
        public Direction Direction { get; set; }
    }
}
