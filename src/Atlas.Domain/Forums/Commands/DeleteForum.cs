using System;
using Docs.Attributes;

namespace Atlas.Domain.Forums.Commands
{
    [DocRequest(typeof(Forum))]
    public class DeleteForum : CommandBase
    {
        public Guid Id { get; set; }
    }
}
