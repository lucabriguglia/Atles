using System;
using Docs.Attributes;

namespace Atlas.Domain.Forums.Commands
{
    [DocRequest(typeof(Forum))]
    public class MoveForum : CommandBase
    {
        public Guid Id { get; set; }
        public Direction Direction { get; set; }
    }
}
