using System;

namespace Atlas.Domain.Topics.Commands
{
    public class UpdateTopic : CommandBase
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public StatusType Status { get; set; }
    }
}
