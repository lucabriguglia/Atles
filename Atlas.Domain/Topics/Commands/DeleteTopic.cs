using System;

namespace Atlas.Domain.Topics.Commands
{
    public class DeleteTopic : CommandBase
    {
        public Guid Id { get; set; }
    }
}
